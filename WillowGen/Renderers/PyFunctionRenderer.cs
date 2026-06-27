using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyFunctionRenderer(FunctionDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        RenderFunctionHeader(sink);
        RenderFunctionParameters(sink);
        RenderFunctionReturnType(sink);
        RenderDocumentation(sink);
    }

    private void RenderFunctionHeader(Sink sink)
    {
        if (elem.HasSparseOptionalParams(e => e.IsOptionalParam && !e.IsOutParam))
        {
            sink.AppendLine("# sparse optional params");
        }
        
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
    }

    private void RenderFunctionParameters(Sink sink)
    {
        var scratch = new StringSink();
        var isFirstParam = elem.IsStatic;

        foreach (var param in elem.Params)
        {
            if (!isFirstParam)
            {
                scratch.Append(", ");
            }

            isFirstParam = false;
            new PyParamRenderer(param, scope).Render(scratch);
        }

        // if there are any invalid overrides or badly named variables, then we force positional
        // only invocation
        if (
            elem.FamilyHasNaughtyOverride
            || elem.Params.Any(p => !PyIdentifier.IsValid(p.Name()))
        )
        {
            scratch.Append(", /");
        }

        sink.AppendRaw(scratch.ToString());
        sink.AppendRaw(") -> ");
    }

    private void RenderFunctionReturnType(Sink sink)
    {
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
                {
                    sink.AppendRaw(", ");
                }

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
        foreach (var line in lines.Select(e => e.TrimEnd()))
        {
            var isBlank = line.Trim().Length == 0;
            if (isBlank && bLastWasBlank)
            {
                continue;
            }

            bLastWasBlank = isBlank;
            sink.AppendLine(line);
        }

        sink.PopIndent();

        sink.AppendLine("\"\"\"");
        sink.PopIndent();
    }
}