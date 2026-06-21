using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        var type = RendererUtils.GetTypeName(elem.ParamType, scope);

        // sanitise only function parameters
        var name = elem.Parent is FunctionDef ? PyIdentifier.Sanitize(elem.Name()) : elem.Name();

        if (elem.Parent is ClassDef cls && (cls.Name() == "Object" || !PyIdentifier.IsValid(name)))
        {
            sink.Append($"# {name}: {type}");
        }
        else
        {
            sink.Append($"{name}: ");

            if (elem.IsOptionalParam)
            {
                sink.AppendRaw("Opt");
            }

            if (elem.IsOutParam)
            {
                sink.AppendRaw("Out");
            }

            if (elem.IsOptionalParam || elem.IsOutParam)
            {
                sink.AppendRaw($"[{type}]");
            }
            else
            {
                sink.AppendRaw(type);

                // if (elem.ParamType is NamedType nt && nt.IsClassRef())
                // {
                //     sink.AppendRaw(" | None");
                // }
            }
        }
    }
}
