using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        var type = RendererUtils.GetTypeName(elem.ParamType, scope);

        if (elem.Parent is ClassDef cls && cls.Name() == "Object")
        {
            sink.Append($"# {elem.Name()}: {type}");
        }
        else
        {
            sink.Append($"{elem.Name()}: {type}");
            if (elem.IsFunctionParam && (elem.IsOutParam || elem.IsOptionalParam))
            {
                sink.AppendRaw(" | None");
            }
        }
    }
}