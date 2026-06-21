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
                sink.AppendLine("# naughty override");

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
            if (!isFirstParam)
                scratch.Append(", ");
            isFirstParam = false;
            RendererUtils.Create(param, scope).Render(scratch);
        }

        if (elem.HasAnyNaughtyOverrides() || elem.Params.Any(p => !PyIdentifier.IsValid(p.Name())))
        {
            scratch.Append(", /");
        }

        sink.AppendRaw(scratch.ToString());
        sink.AppendRaw(") -> ");

        if (elem.HasOutParms)
        {
            var isFirst = elem.ReturnValue == null;
            sink.AppendRaw("tuple[");
            if (elem.ReturnValue != null)
            {
                var retType = RendererUtils.GetReturnTypeName(elem.ReturnValue.ParamType, scope);
                sink.AppendRaw($"{retType}");
            }

            // output parameters are returned directly
            foreach (var param in elem.Params.Where(p => p.IsOutParam))
            {
                if (!isFirst)
                    sink.AppendRaw(", ");
                isFirst = false;
                var paramType = RendererUtils.GetReturnTypeName(param.ParamType, scope);
                sink.AppendRaw(paramType);
            }

            sink.AppendLineRaw("]:");
        }
        else if (elem.ReturnValue != null)
        {
            var retType = RendererUtils.GetReturnTypeName(elem.ReturnValue.ParamType, scope);
            sink.AppendLineRaw($"{retType}:");
        }
        else
        {
            sink.AppendLineRaw("None:");
        }

        RenderDocumentation(sink);
    }

    private void RenderDocumentation(Sink sink)
    {
        sink.PushIndent();
        sink.AppendLine("\"\"\"");
        sink.AppendLine($"Unreal Path: `{elem.Export.GetObjectPath()}`");
        sink.AppendLine();

        sink.AppendLine(".. Decompiled UnrealScript:: c");
        sink.PushIndent();
        var lines = elem.Export.Decompile().Split(Environment.NewLine);
        var bLastWasBlank = false;
        foreach (var line in lines)
        {
            var isBlank = line.Trim().Length == 0;
            if (isBlank && bLastWasBlank)
                continue;
            bLastWasBlank = isBlank;
            sink.AppendLine(line);
        }

        sink.PopIndent();

        sink.AppendLine("\"\"\"");
        sink.PopIndent();
    }
}
