using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyArgumentListRenderer(NamingScope namingScope, ClassDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        foreach (var func in elem.Functions.Where(e => e.Params.Count > 0))
        {
            sink.AppendLine($"class {func.Name()}Args(WrappedStruct):");
            sink.PushIndent();

            var dynamicFields = new List<TypedParamDef>();
            foreach (var param in func.Params)
            {
                if (!PyIdentifier.IsValid(param.Name()))
                {
                    dynamicFields.Add(param);
                }
                else
                {
                    new PyParamRenderer(param, namingScope).RenderMemberVariable(sink);
                    sink.AppendLine();
                }
            }

            // this shit is so fucking rare btw
            if (dynamicFields.Count > 0)
            {
                RenderDynamicAccessors(sink, dynamicFields);
            }

            sink.PopIndent();
            sink.AppendLine();
        }
    }

    private void RenderDynamicAccessors(Sink sink, List<TypedParamDef> fields)
    {
        foreach (var field in fields)
        {
            var type = RendererUtils.GetTypeName(field.ParamType, namingScope);
            sink.AppendLine("@overload");
            sink.AppendLine($"def __getattr__(self, name: Literal[\"{field.Name()}\"]) -> {type}: ...");
        }

        sink.AppendLine("@overload");
        sink.AppendLine("def __getattr__(self, name: str) -> Any: ...");

        foreach (var field in fields)
        {
            var type = RendererUtils.GetTypeName(field.ParamType, namingScope);
            var value = PyParamRenderer.CanNormallyBeNone(field.ParamType) ? $"{type} | None" : type;
            sink.AppendLine("@overload");
            sink.AppendLine($"def __setattr__(self, name: Literal[\"{field.Name()}\"], value: {value}) -> None: ...");
        }

        sink.AppendLine("@overload");
        sink.AppendLine("def __setattr__(self, name: str, value: Any) -> None: ...");
    }
}