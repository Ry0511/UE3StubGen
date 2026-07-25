using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyClassRenderer(string importRoot, ClassDef elem) : IRenderable
{
    private readonly PyImportRenderer _symbols = new(importRoot, elem);

    private NamingScope Scope => _symbols.NameResolver;

    public void Render(Sink sink)
    {
        var scratch = new StringSink(sink);

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
        if (elem.Fields.Count > 0)
        {
            scratch.AppendLine();
        }

        RenderClassFunctions(scratch);
        new PyArgumentListRenderer(_symbols.NameResolver, elem).Render(scratch);
        scratch.PopIndent();

        sink.AppendLineRaw(preface.ToString());
        sink.AppendLineRaw(scratch.ToString());
    }

    private void RenderStructsAndEnums(Sink sink)
    {
        foreach (var e in elem.Enums)
        {
            var renderer = new PyEnumRenderer(e);
            renderer.Render(sink);
            sink.AppendLine();
        }

        foreach (var e in elem.Structs)
        {
            foreach (var child in e.ChildStructs)
            {
                new PyStructRenderer(child, Scope).Render(sink);
                sink.AppendLine();
            }

            new PyStructRenderer(e, Scope).Render(sink);
            sink.AppendLine();
        }

        foreach (var e in elem.Functions.Where(e => e.IsDelegate))
        {
            var renderer = new PyDelegateRenderer(e, Scope);
            renderer.Render(sink);
            sink.AppendLine();
        }
    }

    private void RenderClassHeader(Sink sink)
    {
        sink.Append($"class {elem.Name()}");

        var scratch = new StringSink(sink);

        scratch.Append(
            elem.Super != null && elem.Name() != "Interface"
                ? $"({RendererUtils.GetRefTypeName(elem.Super, Scope)}"
                : "(UObject");

        foreach (var iface in elem.Interfaces)
        {
            scratch.Append(", ");
            scratch.Append(RendererUtils.GetRefTypeName(iface, Scope));
        }

        sink.AppendLineRaw(scratch + "):");
    }

    private void RenderClassFields(Sink sink)
    {
        var scratch = new StringSink();
        foreach (var field in elem.Fields)
        {
            scratch.Clear();
            if (field.ParamType is DelegateType)
            {
                scratch.Append("# ");
            }

            new PyParamRenderer(field, Scope).Render(scratch);
            sink.AppendLine(scratch.ToString());
        }
    }

    private void RenderClassFunctions(Sink sink)
    {
        var functionDecl = new StringSink(sink);
        foreach (var func in elem.Functions)
        {
            functionDecl.Reset(sink);
            new PyFunctionRenderer(func, Scope).Render(functionDecl);
            sink.AppendLineRaw(functionDecl.ToString());
            if (func != elem.Functions[^1])
            {
                sink.AppendLine();
            }
        }
    }

    private void RenderImportAndPrefaceDefinitions(Sink sink)
    {
        _symbols.RenderDefaultImports(sink);
        _symbols.RenderExternalImports(sink);
    }
}