using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyClassRenderer(ClassDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.Append($"class {elem.Name()}");

        var scratch = new StringSink(sink);
        scratch.Append(
            elem.Super != null
                ? $"({RendererUtils.GetRefTypeName(elem.Super)}"
                : "(UObject"
        );

        foreach (var iface in elem.Interfaces)
        {
            scratch.Append(", ");
            scratch.Append(RendererUtils.GetRefTypeName(iface));
        }

        sink.AppendLineRaw(scratch + "):");

        sink.PushIndent();

        // no fields or functions so just quick exit
        if (elem.Fields.Count == 0 && elem.Functions.Count == 0)
        {
            sink.Append("...");
            return;
        }

        // render all fields
        foreach (var field in elem.Fields)
        {
            scratch.Reset(sink);
            RendererUtils.Create(field).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());
        }

        if (elem.Fields.Count > 0) sink.AppendLine();

        // render functions
        foreach (var func in elem.Functions)
        {
            scratch.Reset(sink);
            RendererUtils.Create(func).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());
            if (func != elem.Functions[^1]) sink.AppendLine();
        }

        sink.PopIndent();
    }
}