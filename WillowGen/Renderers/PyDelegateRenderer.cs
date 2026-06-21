using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyDelegateRenderer(FunctionDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine($"class {RendererUtils.CreateDelegateSignature(elem, scope)}(Protocol):");
        sink.PushIndent();

        sink.Append("def __call__(self");

        var scratch = new StringSink();
        foreach (var param in elem.Params)
        {
            scratch.Append(", ");
            RendererUtils.Create(param, scope).Render(scratch);
        }

        if (elem.Params.Any(p => !PyIdentifier.IsValid(p.Name())))
        {
            scratch.Append(", /");
        }

        sink.AppendRaw(scratch.ToString());

        sink.AppendLineRaw(
            elem.ReturnValue != null
                ? $") -> {RendererUtils.GetTypeName(elem.ReturnValue!.ParamType, scope)}: ..."
                : ") -> None: ...");
        sink.PopIndent();
    }
}
