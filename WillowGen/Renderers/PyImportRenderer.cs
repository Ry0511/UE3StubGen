using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyImportRenderer : IRenderable
{
    public NamingScope NameResolver { get; private set; } = NamingScope.Empty;

    private readonly string _importRoot;
    private readonly Dictionary<string, BaseSymbol?> _namedTypes = new();
    private readonly Dictionary<string, BaseSymbol?> _internalNamedTypes = new();
    private readonly Dictionary<string, string> _localNames = new();
    private readonly HashSet<string> _reservedNames;
    private readonly ClassDef _elem;

    public PyImportRenderer(string importRoot, ClassDef elem)
    {
        _elem = elem;
        _importRoot = importRoot.Length == 0 ? string.Empty : importRoot + '.';
        _reservedNames =
        [
            elem.Name(),
            "name",
            "byte",
            "Unresolved",
            "Opt",
            "Out",
            "Delegate",
            "Any",
            "Protocol",
            "override",
            "Literal",
            "UObject",
            "UClass",
            "WrappedArray",
            "WrappedStruct"
        ];
        CollectNames();
        BuildNamingScope();
    }

    public void Render(Sink sink) => RenderExternalImports(sink);

    public void RenderExternalImports(Sink sink) => RenderImports(sink, _namedTypes);

    public void RenderInternalImports(Sink sink) => RenderImports(sink, _internalNamedTypes);

    private void RenderImports(Sink sink, IReadOnlyDictionary<string, BaseSymbol?> types)
    {
        var imports = new SortedDictionary<string, SortedSet<string>>(StringComparer.Ordinal);
        foreach (var (path, ty) in types.Where(e => e.Value != null))
        {
            var module = $"{_importRoot}{ty!.Module!.Name()}";

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

    public void RenderDefaultImports(Sink sink)
    {
        sink.AppendLine("from enum import IntEnum");
        sink.AppendLine("from _typeshed import MaybeNone, sentinel");
        sink.AppendLine("from types import EllipsisType");
        sink.AppendLine("from typing import Any, Protocol, override, Literal, overload");
        sink.AppendLine("from enum import auto");
        sink.AppendLine("from unrealsdk.unreal import UObject, UClass, WrappedArray, WrappedStruct");
        sink.AppendLine("from unrealsdk.unreal._uenum import UnrealEnum");
        sink.AppendLine($"from {_importRoot}stubgenapi import name, byte, Out, AcceptsNone, Delegate, Unresolved, bound_function, delegate");
    }

    public static string LocalBaseName(BaseSymbol ty) => ty.Name();

    private void CollectNames()
    {
        foreach (var imp in _elem.Descendants().OfType<RefNode>())
        {
            if (imp.ResolvedTo == null)
            {
                _namedTypes[imp.TargetFullPath] = null;
                continue;
            }

            if (imp.Ancestors().OfType<ClassType>().Any())
            {
                continue;
            }

            var owner = imp.ResolvedTo!.Ancestors(true).OfType<ClassDef>().FirstOrDefault();
            if (owner == null || owner != _elem)
            {
                _namedTypes[imp.TargetFullPath] = imp.ResolvedTo;
            }
            else
            {
                _internalNamedTypes[imp.TargetFullPath] = imp.ResolvedTo;
            }
        }
    }

    private void BuildNamingScope()
    {
        foreach (var field in _elem.Fields)
        {
            _reservedNames.Add(field.Name());
        }

        foreach (var func in _elem.Functions)
        {
            _reservedNames.Add(LocalBaseName(func));
        }

        foreach (var @enum in _elem.Enums)
        {
            _reservedNames.Add(@enum.Name());
        }

        foreach (var @struct in _elem.Structs)
        {
            _reservedNames.Add(@struct.Name());
            foreach (var child in @struct.ChildStructs)
            {
                _reservedNames.Add(child.Name());
            }
        }

        var taken = new HashSet<string>(_reservedNames, StringComparer.Ordinal);
        foreach (var (_, ty) in _namedTypes.Where(e => e.Value != null))
        {
            taken.Add(LocalBaseName(ty!));
        }

        foreach (var (path, ty) in _namedTypes.Where(e => e.Value != null))
        {
            var name = LocalBaseName(ty!);
            if (!_reservedNames.Contains(name))
            {
                continue;
            }

            var alias = name + 'A';
            for (var c = 'B'; c != 'Z'; c++)
            {
                if (taken.Add(alias))
                {
                    break;
                }

                alias = name + c;
            }

            _localNames[path] = alias;
        }

        NameResolver = new NamingScope(_localNames);
    }
}