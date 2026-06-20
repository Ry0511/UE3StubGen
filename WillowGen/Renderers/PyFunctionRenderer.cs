using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyFunctionRenderer(FunctionDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        if (elem.IsOverride)
        {
            if (elem.IsNaughtyOverride)
            {
                sink.AppendLine("# naughty override");
            }

            sink.AppendLine("@override");
        }

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
            RendererUtils.Create(param, scope).Render(scratch);
        }

        if (elem.Overriders.Any(e => e.IsNaughtyOverride))
        {
            scratch.Append(", /");
        }

        sink.AppendRaw(scratch.ToString());

        sink.AppendLineRaw(
            elem.ReturnValue != null
                ? $") -> {RendererUtils.GetTypeName(elem.ReturnValue!.ParamType, scope)}:"
                : ") -> None:"
        );

        RenderDocumentation(sink);
    }

    private void RenderDocumentation(Sink sink)
    {
        sink.PushIndent();
        sink.AppendLine("\"\"\"");
        sink.AppendLine($"Unreal Path: `{elem.Export.GetObjectPath()}`");
        sink.AppendLine();

        // state all the optional/out parameters up front
        if (elem.HasOptionalParms || elem.HasOutParms)
        {
            sink.AppendLine(".. Optional/Output Parameters:: text");
            sink.PushIndent();
            foreach (var param in elem.Params.Where(p => p.IsOptionalParam || p.IsOutParam))
            {
                var modifiers = "";
                if (param.IsOptionalParam) modifiers += "optional ";
                if (param.IsOutParam) modifiers += "out ";
                sink.AppendLine($"{modifiers}{param.Name()}");
            }

            sink.PopIndent();
            sink.AppendLine();
        }

        // Render the script body as a code block, use C style highlighting when supported
        sink.AppendLine(".. Decompiled UnrealScript:: c");
        sink.PushIndent();
        var lines = elem.Export.Decompile().Split(Environment.NewLine);
        var bLastWasBlank = false;
        foreach (var line in lines)
        {
            var isBlank = line.Trim().Length == 0;
            if (isBlank && bLastWasBlank) continue;
            bLastWasBlank = isBlank;
            sink.AppendLine(line);
        }

        sink.PopIndent();

        sink.AppendLine("\"\"\"");
        sink.PopIndent();
    }
}