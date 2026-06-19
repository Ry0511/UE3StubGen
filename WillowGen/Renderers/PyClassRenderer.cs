using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyClassRenderer(ClassDef elem) : IRenderable
{
    private readonly Dictionary<string, BaseSymbol?> _namedTypes = new();

    public void Render(Sink sink)
    {
        var scratch = new StringSink(sink);

        RenderStructsAndEnums(scratch);

        RenderClassHeader(scratch);
        scratch.PushIndent();

        if (elem.Fields.Count == 0 && elem.Functions.Count == 0)
        {
            scratch.Append("...");
            sink.AppendLineRaw(scratch.ToString());
            return;
        }

        RenderClassFields(scratch);
        if (elem.Fields.Count > 0) scratch.AppendLine();

        RenderClassFunctions(scratch);
        scratch.PopIndent();

        var preface = new StringSink(sink);

        foreach (var imp in elem.Descendants().OfType<RefNode>())
        {
            if (
                imp.ResolvedTo == null
                || imp.ResolvedTo
                    .Ancestors(includeSelf: true)
                    .OfType<ClassDef>()
                    .All(e => e.Name() != elem.Name())
            )
            {
                _namedTypes[imp.TargetFullPath] = imp.ResolvedTo;
            }
        }

        RenderImportAndPrefaceDefinitions(preface);

        sink.AppendLineRaw(preface.ToString());
        sink.AppendLineRaw(scratch.ToString());
    }

    private void RenderStructsAndEnums(Sink sink)
    {
        foreach (var e in elem.Enums)
        {
            var renderer = RendererUtils.Create(e);
            renderer.Render(sink);
            sink.AppendLine();
        }

        foreach (var e in elem.Structs)
        {
            var renderer = RendererUtils.Create(e);
            renderer.Render(sink);
            sink.AppendLine();
        }
    }

    private void RenderClassHeader(Sink sink)
    {
        sink.Append($"class {elem.Name()}");

        if (elem.Name() != "Interface")
        {
            var scratch = new StringSink(sink);

            if (elem.Super != null)
            {
                _namedTypes[elem.Super.TargetFullPath] = elem.Super.ResolvedTo;
            }

            scratch.Append(
                elem.Super != null
                    ? $"({RendererUtils.GetRefTypeName(elem.Super)}"
                    : "(UObject"
            );

            foreach (var iface in elem.Interfaces)
            {
                scratch.Append(", ");
                scratch.Append(RendererUtils.GetRefTypeName(iface));
                _namedTypes[iface.TargetFullPath] = iface.ResolvedTo;
            }
            
            sink.AppendLineRaw(scratch + "):");
        }
        else
        {
            sink.AppendLineRaw(":");
        }
        
    }

    private void RenderClassFields(Sink sink)
    {
        var scratch = new StringSink(sink);
        foreach (var field in elem.Fields)
        {
            scratch.Reset(sink);
            RendererUtils.Create(field).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());
        }
    }

    private void RenderClassFunctions(Sink sink)
    {
        var scratch = new StringSink(sink);
        foreach (var func in elem.Functions)
        {
            scratch.Reset(sink);
            RendererUtils.Create(func).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());
            if (func != elem.Functions[^1]) sink.AppendLine();
        }
    }

    private void RenderImportAndPrefaceDefinitions(Sink sink)
    {
        sink.AppendLine("from enum import IntEnum");
        sink.AppendLine("from typing import Any, Generic, TypeVar");
        sink.AppendLine("from unrealsdk.unreal import UObject, UClass, WrappedArray, WrappedStruct");

        // Core.Object
        // WillowGame.WillowPlayerController
        // from Core.Object import Object, Pointer
        foreach (var (_, ty) in _namedTypes.Where(e => e.Value != null))
        {
            sink.Append($"from bl1.{ty!.Module!.Name()}");

            if (ty is ClassDef cls)
            {
                sink.AppendRaw($".{cls.Name()} import {cls.Name()}");
            }
            else
            {
                var parent = ty.Parent as ClassDef;
                sink.AppendRaw($".{parent!.Name()} import {ty.Name()}");
            }

            sink.AppendLine();
        }

        sink.AppendLine();
        sink.AppendLine("type name = str");
        sink.AppendLine("type byte = int");

        foreach (var (path, _) in _namedTypes.Where(e => e.Value == null))
        {
            sink.AppendLine($"type {path.Split(".").Last()} = Any");
        }

        sink.AppendLine("_InternalGenericType = TypeVar(\"_InternalGenericType\")");
        sink.AppendLine();
        sink.AppendLine();
        sink.AppendLine("class Struct(Generic[_InternalGenericType]): ...");
        sink.AppendLine();
        sink.AppendLine();
        sink.AppendLine("class Enum(Generic[_InternalGenericType]): ...");

        sink.AppendLine();
    }
}