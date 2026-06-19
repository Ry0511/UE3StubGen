using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyDelegateRenderer(FunctionDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine($"class _delegate_{elem.Name()}(Protocol):");
        sink.PushIndent();

        sink.Append("def __call__(self");

        var scratch = new StringSink();
        foreach (var param in elem.Params)
        {
            scratch.Append(", ");
            RendererUtils.Create(param).Render(scratch);
        }

        sink.AppendRaw(scratch.ToString());

        sink.AppendLineRaw(
            elem.ReturnValue != null
                ? $") -> {RendererUtils.GetTypeName(elem.ReturnValue!.ParamType)}: ..."
                : ") -> None: ..."
        );
        sink.PopIndent();
    }
}