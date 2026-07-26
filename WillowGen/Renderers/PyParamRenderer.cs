using System.Text;
using UE3StubGenCore.ASG.Defs;
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
        var types = new PyTypeRenderer(scope);
        var name = PyIdentifier.Sanitize(elem.Name());

        sink.Append($"{name}: ");

        var typeName = new StringBuilder();

        if (elem.IsOutParam)
        {
            typeName.Append("Out[");
        }

        if (elem.IsOptionalParam || elem.IsOutParam)
        {
            typeName.Append(types.RenderRaw(elem.ParamType));
            if (elem.IsArray && elem.IsOutParam)
            {
                typeName.Append(" | list[None]");
            }
        }
        else
        {
            typeName.Append(types.RenderFunctionParam(elem.ParamType));
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
        var types = new PyTypeRenderer(scope);
        var name = elem.Name();

        if (elem.Parent is ClassDef cls && (cls.Name() == "Object" || !PyIdentifier.IsValid(name)))
        {
            sink.Append($"# {name}: {types.RenderRaw(elem.ParamType)}");
            return;
        }

        sink.Append($"{name}: {types.RenderMemberVariable(elem.ParamType)}");
    }

    public static bool IsTrueOptional(TypedParamDef elem) => elem.IsOptionalParam && !elem.IsOutParam;

    public bool IsTrueOptional() => IsTrueOptional(elem);
}
