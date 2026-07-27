using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyArgumentListRenderer(NamingScope namingScope, ClassDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        foreach (var func in elem.Functions.Where(e => e.Params.Count > 0))
        {
            sink.AppendLine($"class {func.Name()}Args(WrappedStruct):");
            sink.PushIndent();
            int renderCount = 0;

            foreach (var param in func.Params)
            {
                if (!PyIdentifier.IsValid(param.Name()))
                {
                    var scratch = new StringSink();
                    new PyParamRenderer(param, namingScope).Render(scratch);
                    sink.AppendLine("# " + scratch);
                }
                else
                {
                    new PyParamRenderer(param, namingScope).RenderFunctionParam(sink);
                    sink.AppendLine();
                    ++renderCount;
                }
            }

            if (renderCount == 0)
            {
                sink.AppendLine("...");
            }

            sink.PopIndent();
            sink.AppendLine();
        }
    }
}