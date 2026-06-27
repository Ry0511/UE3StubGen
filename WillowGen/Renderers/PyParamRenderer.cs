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

            if (elem.IsOutParam)
            {
                sink.AppendRaw("Out[");
            }

            if (elem.IsOptionalParam || elem.IsOutParam)
            {
                sink.AppendRaw(type);
                if (elem.IsArray && elem.IsOutParam)
                {
                    sink.AppendRaw(" | list[None]");
                }
            }
            else
            {
                sink.AppendRaw(type);
            }

            // I did think about pulling the default value from the property, but that seems to
            // require probing the bytecode, which is just not worth it. The decompiled script code
            // shows you the default already.
            if (elem.IsFunctionParam && IsTrueOptional())
            {
                sink.AppendRaw(" | None = None");
            }
            else if (CanNormallyBeNone())
            {
                sink.AppendRaw(" | None");
            }

            if (elem.IsOutParam)
            {
                sink.AppendRaw("]");
            }
        }
    }

    public static bool IsTrueOptional(TypedParamDef elem) => elem.IsOptionalParam && !elem.IsOutParam;

    public bool IsTrueOptional() => IsTrueOptional(elem);

    public bool CanNormallyBeNone()
    {
        return elem.ParamType switch
        {
            ClassType _ => true,
            DelegateType _ => true,
            DynArrayType _ => false,
            EngineBuiltinType _ => false,
            InterfaceType _ => true,
            NamedType e => e.IsClassRef(),
            StaticArrayType _ => false,
            UnhandledType _ => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}