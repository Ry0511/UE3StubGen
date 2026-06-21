using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyStructRenderer(StructDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.Append($"class {elem.Name()}");
        sink.AppendLineRaw(
            elem.Super == null
                ? "(WrappedStruct):"
                : $"({RendererUtils.GetRefTypeName(elem.Super, scope)}):");

        sink.PushIndent();

        foreach (var field in elem.Fields)
        {
            var scratch = new StringSink();
            new PyParamRenderer(field, scope).Render(scratch);
            sink.AppendLine(scratch.ToString());
        }

        if (elem.Fields.Count == 0)
        {
            sink.Append("...");
        }

        sink.PopIndent();
    }
}
