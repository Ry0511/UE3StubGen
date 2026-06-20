using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        if (elem.Parent is ClassDef cls && cls.Name() == "Object")
        {
            sink.Append($"# {elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType, scope)}");
        }
        else if (elem.IsFunctionParam)
        {
            // TODO: try to simplify this a bit since its all the same
            if (elem.IsOptionalParam && elem.IsOutParam)
            {
                sink.Append($"{elem.Name()}: OptOut[{RendererUtils.GetTypeName(elem.ParamType, scope)}]");
            }
            else if (elem.IsOptionalParam)
            {
                sink.Append($"{elem.Name()}: Opt[{RendererUtils.GetTypeName(elem.ParamType, scope)}]");
            }
            else if (elem.IsOutParam)
            {
                sink.Append($"{elem.Name()}: Out[{RendererUtils.GetTypeName(elem.ParamType, scope)}]");
            }
            else
            {
                sink.Append($"{elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType, scope)}");
            }
        }
        else
        {
            sink.Append($"{elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType, scope)}");
        }
    }
}