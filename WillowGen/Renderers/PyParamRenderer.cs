using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.Append(elem.Name());
    }
}