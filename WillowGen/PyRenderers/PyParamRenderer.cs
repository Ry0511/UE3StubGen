using UE3StubGenCore.ASG.Defs;
using WillowGen.Renderer;

namespace WillowGen.PyRenderers;

public class PyParamRenderer(TypedParamDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.Append(elem.Name());
    }
}