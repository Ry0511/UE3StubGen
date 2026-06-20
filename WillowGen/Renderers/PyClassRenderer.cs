using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyClassRenderer(ClassDef elem) : IRenderable
{
    private readonly Dictionary<string, BaseSymbol?> _namedTypes = new();
    private readonly Dictionary<string, string> _localNames = new();
    private NamingScope _scope = NamingScope.Empty;

    public void Render(Sink sink)
    {
        var scratch = new StringSink(sink);

        // collect every referenced type up front so import aliases are known before we render hints
        foreach (var imp in elem.Descendants().OfType<RefNode>())
        {
            if (imp.ResolvedTo == null)
            {
                _namedTypes[imp.TargetFullPath] = null;
                continue;
            }

            // no need to import the metaclass name i.e., class<DamageType>
            if (imp.Ancestors().OfType<ClassType>().Any())
            {
                continue;
            }

            // external import
            var owner = imp.ResolvedTo!.Ancestors(true).OfType<ClassDef>().FirstOrDefault();
            if (owner == null || owner != elem)
            {
                _namedTypes[imp.TargetFullPath] = imp.ResolvedTo;
            }
        }

        BuildNamingScope();
        RenderStructsAndEnums(scratch);

        RenderClassHeader(scratch);
        scratch.PushIndent();

        var preface = new StringSink(sink);
        RenderImportAndPrefaceDefinitions(preface);

        if (elem.Fields.Count == 0 && elem.Functions.Count == 0)
        {
            scratch.Append("...");
            sink.AppendLineRaw(preface.ToString());
            sink.AppendLineRaw(scratch.ToString());
            return;
        }

        RenderClassFields(scratch);
        if (elem.Fields.Count > 0) scratch.AppendLine();

        RenderClassFunctions(scratch);
        scratch.PopIndent();

        sink.AppendLineRaw(preface.ToString());
        sink.AppendLineRaw(scratch.ToString());
    }

    private static string LocalBaseName(BaseSymbol ty)
    {
        return ty is FunctionDef { IsDelegate: true } func
            ? RendererUtils.CreateDelegateSignature(func)
            : ty.Name();
    }

    private void BuildNamingScope()
    {
        var reserved = new HashSet<string>(StringComparer.Ordinal) { elem.Name() };

        // build up the reserved names that cannot change
        foreach (var field in elem.Fields) reserved.Add(field.Name());
        foreach (var func in elem.Functions) reserved.Add(LocalBaseName(func));
        foreach (var @enum in elem.Enums) reserved.Add(@enum.Name());
        foreach (var @struct in elem.Structs)
        {
            reserved.Add(@struct.Name());
            foreach (var child in @struct.ChildStructs)
            {
                reserved.Add(child.Name());
            }
        }

        // aliases must also avoid colliding with other imports' original names
        var taken = new HashSet<string>(reserved, StringComparer.Ordinal);
        foreach (var (_, ty) in _namedTypes.Where(e => e.Value != null))
        {
            taken.Add(LocalBaseName(ty!));
        }

        foreach (var (path, ty) in _namedTypes.Where(e => e.Value != null))
        {
            var name = LocalBaseName(ty!);
            if (!reserved.Contains(name)) continue;

            // Name -> NameA, NameB, ... (realistically never exceeds one or two)
            var alias = name + 'A';
            for (var c = 'B'; c != 'Z'; c++)
            {
                if (taken.Add(alias)) break;
                alias = name + c;
            }

            _localNames[path] = alias;
        }

        _scope = new NamingScope(_localNames);
    }

    private void RenderStructsAndEnums(Sink sink)
    {
        foreach (var e in elem.Enums)
        {
            var renderer = RendererUtils.Create(e, _scope);
            renderer.Render(sink);
            sink.AppendLine();
        }

        foreach (var e in elem.Structs)
        {
            // literally only exists because of one struct deciding to be different
            foreach (var child in e.ChildStructs)
            {
                new PyStructRenderer(child, _scope).Render(sink);
                sink.AppendLine();
            }

            new PyStructRenderer(e, _scope).Render(sink);
            sink.AppendLine();
        }

        foreach (var e in elem.Functions.Where(e => e.IsDelegate))
        {
            var renderer = RendererUtils.Create(e, _scope);
            renderer.Render(sink);
            sink.AppendLine();
        }
    }

    private void RenderClassHeader(Sink sink)
    {
        sink.Append($"class {elem.Name()}");

        var scratch = new StringSink(sink);

        if (elem.Super != null) _namedTypes[elem.Super.TargetFullPath] = elem.Super.ResolvedTo;

        scratch.Append(
            elem.Super != null
                ? $"({RendererUtils.GetRefTypeName(elem.Super, _scope)}"
                : "(UObject"
        );

        foreach (var iface in elem.Interfaces)
        {
            scratch.Append(", ");
            scratch.Append(RendererUtils.GetRefTypeName(iface, _scope));
            _namedTypes[iface.TargetFullPath] = iface.ResolvedTo;
        }

        sink.AppendLineRaw(scratch + "):");
    }

    private void RenderClassFields(Sink sink)
    {
        var scratch = new StringSink();
        foreach (var field in elem.Fields)
        {
            scratch.Clear();
            if (field.ParamType is DelegateType) scratch.Append("# ");
            new PyParamRenderer(field, _scope).Render(scratch);
            sink.AppendLine(scratch.ToString());
        }
    }

    private void RenderClassFunctions(Sink sink)
    {
        var scratch = new StringSink(sink);
        foreach (var func in elem.Functions)
        {
            scratch.Reset(sink);
            new PyFunctionRenderer(func, _scope).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());
            if (func != elem.Functions[^1]) sink.AppendLine();
        }
    }

    private void RenderImportAndPrefaceDefinitions(Sink sink)
    {
        sink.AppendLine("from enum import IntEnum");
        sink.AppendLine("from typing import Any, Protocol, override");
        sink.AppendLine("from unrealsdk.unreal import UObject, UClass, WrappedArray, WrappedStruct");
        sink.AppendLine("from bl1.stubgenapi import name, byte, Opt, Out, OptOut, UnresolvedClass");

        var imports = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        foreach (var (path, ty) in _namedTypes.Where(e => e.Value != null))
        {
            var module = $"bl1.{ty!.Module!.Name()}";

            // not exported to the module so need to go a level deeper
            if (!ty.IsModuleUnique)
            {
                var cls = ty.Ancestors().OfType<ClassDef>().FirstOrDefault();
                module += '.' + cls!.Name();
            }

            var name = LocalBaseName(ty);
            var local = _localNames.GetValueOrDefault(path, name);
            var symbol = local == name ? name : $"{name} as {local}";

            if (!imports.TryGetValue(module, out var symbols))
            {
                imports[module] = symbols = new SortedSet<string>(StringComparer.Ordinal);
            }

            symbols.Add(symbol);
        }

        foreach (var (module, symbols) in imports)
        {
            sink.AppendLine($"from {module} import {string.Join(", ", symbols)}");
        }
    }
}