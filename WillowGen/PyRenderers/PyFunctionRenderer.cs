using UE3StubGenCore.ASG.Defs;
using WillowGen.Renderer;

namespace WillowGen.PyRenderers;

public class PyFunctionRenderer(FunctionDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        // TODO: parameter types and return type

        if (elem.IsStatic)
        {
            sink.AppendLine("@staticmethod");
            sink.Append($"def {elem.Name()}(");
        }
        else
        {
            sink.Append($"def {elem.Name()}(self");
        }

        var scratch = new StringSink();
        bool isFirstParam = elem.IsStatic;
        foreach (var param in elem.Params)
        {
            if (!isFirstParam)
            {
                scratch.Append(", ");
            }
            else
            {
                isFirstParam = false;
            }

            RendererUtils.Create(param).Render(scratch);
        }

        sink.AppendRaw(scratch.ToString());
        sink.AppendRaw("): ...");
    }
}