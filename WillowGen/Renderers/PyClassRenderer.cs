using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyClassRenderer(ClassDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine($"class {elem.Name()}:");

        // TODO: inheritance list

        sink.PushIndent();

        // no fields or functions so just quick exit
        if (elem.Fields.Count == 0 && elem.Functions.Count == 0)
        {
            sink.Append("...");
            return;
        }

        var scratch = new StringSink(sink);

        // render all fields
        foreach (var field in elem.Fields)
        {
            scratch.Reset(sink);
            RendererUtils.Create(field).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());
        }

        if (elem.Fields.Count > 0) sink.AppendLine();

        // render functions
        var lastWasStatic = false;
        foreach (var func in elem.Functions)
        {
            scratch.Reset(sink);
            RendererUtils.Create(func).Render(scratch);
            sink.AppendLineRaw(scratch.ToString());

            if (lastWasStatic && !func.IsStatic) sink.AppendLine();
            lastWasStatic = func.IsStatic;
        }

        sink.PopIndent();
    }
}