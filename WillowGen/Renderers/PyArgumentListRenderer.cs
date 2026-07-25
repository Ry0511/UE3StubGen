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

            foreach (var param in func.Params)
            {
                new PyParamRenderer(param, namingScope).Render(sink);
                sink.AppendLine();
            }

            sink.PopIndent();
            sink.AppendLine();
        }
    }
}