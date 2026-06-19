using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyFunctionRenderer(FunctionDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        if (elem.IsOverride) sink.AppendLine("@override");

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
        var isFirstParam = elem.IsStatic;
        foreach (var param in elem.Params)
        {
            if (!isFirstParam) scratch.Append(", ");
            isFirstParam = false;
            RendererUtils.Create(param).Render(scratch);
        }

        sink.AppendRaw(scratch.ToString());

        sink.AppendLineRaw(
            elem.ReturnValue != null
                ? $") -> {RendererUtils.GetTypeName(elem.ReturnValue!.ParamType)}:"
                : ") -> None:"
        );

        sink.PushIndent();
        sink.AppendLine("\"\"\"");

        sink.AppendLine($"object path {elem.Export.GetObjectPath()}");
        sink.AppendLine();

        sink.AppendLine(".. code-block:: text");
        sink.AppendLine();
        sink.PushIndent();
        
        var lines = elem.Export.Decompile().Split(Environment.NewLine);
        foreach (var line in lines)
        {
            sink.AppendLine(line);
        }

        sink.PopIndent();

        sink.AppendLine("\"\"\"");
        sink.PopIndent();
    }
}