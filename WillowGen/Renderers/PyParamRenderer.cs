using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
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
            sink.Append($"{elem.Name()}: ");

            if (elem.IsOptionalParam) sink.AppendRaw("Opt");
            if (elem.IsOutParam) sink.AppendRaw("Out");

            if (elem.IsOptionalParam || elem.IsOutParam)
            {
                sink.AppendRaw($"[{type}]");
            }
            else
            {
                sink.AppendRaw(type);
                if (elem.ParamType is NamedType nt && nt.IsClassRef())
                {
                    sink.AppendRaw(" | None");
                }
            }
        }
    }
}