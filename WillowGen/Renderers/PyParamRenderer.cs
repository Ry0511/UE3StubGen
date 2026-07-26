using System.Text;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        if (elem.IsFunctionParam)
        {
            RenderFunctionParam(sink);
        }
        else
        {
            RenderMemberVariable(sink);
        }
    }

    public void RenderFunctionParam(Sink sink)
    {
        var type = RendererUtils.GetTypeName(elem.ParamType, scope);
        var name = PyIdentifier.Sanitize(elem.Name());

        sink.Append($"{name}: ");

        var typeName = new StringBuilder();

        if (elem.IsOutParam)
        {
            typeName.Append("Out[");
        }

        if (elem.IsOptionalParam || elem.IsOutParam)
        {
            typeName.Append(type);
            if (elem.IsArray && elem.IsOutParam)
            {
                typeName.Append(" | list[None]");
            }
        }
        else
        {
            typeName.Append(type);
        }

        // I did think about pulling the default value from the property, but that seems to
        // require probing the bytecode, which is just not worth it. The decompiled script code
        // shows you the default already.
        if (IsTrueOptional())
        {
            typeName.Append(" = sentinel");
        }

        if (elem.IsOutParam)
        {
            typeName.Append(']');
        }

        sink.AppendRaw(typeName.ToString());
    }

    public void RenderMemberVariable(Sink sink)
    {
        var type = RendererUtils.GetTypeName(elem.ParamType, scope);
        var name = elem.Name();

        if (elem.Parent is ClassDef cls && (cls.Name() == "Object" || !PyIdentifier.IsValid(name)))
        {
            sink.Append($"# {name}: {type}");
            return;
        }

        var rendered = CanNormallyBeNone(elem.ParamType) ? $"AcceptsNone[{type}]" : type;
        sink.Append($"{name}: {rendered}");
    }

    public static bool IsTrueOptional(TypedParamDef elem) => elem.IsOptionalParam && !elem.IsOutParam;

    public bool IsTrueOptional() => IsTrueOptional(elem);

    public static bool CanNormallyBeNone(BaseType ty)
    {
        // TODO: find a place for this
        return ty switch
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