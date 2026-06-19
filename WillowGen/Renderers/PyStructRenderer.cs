using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyStructRenderer(StructDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        // TODO: inheritance list
        sink.Append($"class {sink}");
        sink.AppendLineRaw(elem.Super == null ? "(unrealsdk.unreal.WrappedStruct):" : ":");

        sink.PushIndent();

        foreach (var field in elem.Fields)
        {
            var scratch = new StringSink();
            RendererUtils.Create(field).Render(scratch);
            sink.AppendLine(scratch.ToString());
        }

        sink.PopIndent();
    }
}