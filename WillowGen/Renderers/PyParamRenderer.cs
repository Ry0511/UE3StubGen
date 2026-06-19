using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        if (elem.Parent is ClassDef cls && cls.Name() == "Object")
        {
            sink.Append($"# {elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType)}");
        }
        else
        {
            sink.Append($"{elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType)}");
        }
    }
}