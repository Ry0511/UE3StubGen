using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyFunctionRenderer(FunctionDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        RenderFunctionHeader(sink, elem.Name(), "@bound_function");
        RenderFunctionParameters(sink);
        RenderFunctionReturnType(sink);
        RenderDocumentation(sink);
    }

    public void RenderDelegate(Sink sink, TypedParamDef param)
    {
        RenderFunctionHeader(sink, param.Name(), "@delegate");
        RenderFunctionParameters(sink);
        RenderFunctionReturnType(sink);
        sink.PushIndent();
        sink.AppendLine("pass");
        sink.PopIndent();
    }

    private void RenderFunctionHeader(Sink sink, string name, string? decorator = null)
    {
        List<string> comments = new();

        if (elem.HasSparseOptionalParams(PyParamRenderer.IsTrueOptional))
        {
            comments.Add("sparse optional/out params");
        }

        if (elem.IsOverride)
        {
            comments.Add(elem.IsNaughtyOverride ? "naughty override" : "override");
        }

        if (elem.IsStatic)
        {
            comments.Add("static");
        }

        if (comments.Count > 0)
        {
            sink.AppendLine("# " + string.Join(", ", comments));
        }

        if (decorator != null)
        {
            sink.AppendLine(decorator);
        }

        sink.Append($"def {name}(self");
    }

    private void RenderFunctionParameters(Sink sink)
    {
        var scratch = new StringSink();

        var forceKeywordOnly = elem.HasSparseOptionalParams(PyParamRenderer.IsTrueOptional);
        if (forceKeywordOnly)
        {
            scratch.Append(", *");
        }

        foreach (var param in elem.Params)
        {
            scratch.Append(", ");
            new PyParamRenderer(param, scope).RenderFunctionParam(scratch);
        }

        // if there are any invalid overrides or badly named variables, then we force positional
        // only invocation
        if (
            !forceKeywordOnly
            && (elem.FamilyHasNaughtyOverride
                || elem.Params.Any(p => !PyIdentifier.IsValid(p.Name())))
        )
        {
            scratch.Append(", /");
        }
        else if (elem.Params.Count > 2)
        {
            scratch.Append(",");
        }

        sink.AppendRaw(scratch.ToString());
        sink.AppendRaw(") -> ");
    }

    private void RenderFunctionReturnType(Sink sink)
    {
        var types = new PyTypeRenderer(scope);

        if (elem.HasOutParms)
        {
            var hasMultipleReturns = (elem.ReturnValue != null ? 1 : 0)
                + elem.Params.Count(p => p.IsOutParam) > 1;
            var isFirst = elem.ReturnValue == null;
            if (hasMultipleReturns)
            {
                sink.AppendRaw("tuple[");
            }

            if (elem.ReturnValue != null)
            {
                sink.AppendRaw(types.RenderFunctionReturn(elem.ReturnValue.ParamType));
            }

            // output parameters are returned directly
            foreach (var param in elem.Params.Where(p => p.IsOutParam))
            {
                if (!isFirst)
                {
                    sink.AppendRaw(", ");
                }

                isFirst = false;
                sink.AppendRaw(types.RenderRawReturn(param.ParamType));
            }

            sink.AppendLineRaw(hasMultipleReturns ? "]:" : ":");
        }
        else if (elem.ReturnValue != null)
        {
            sink.AppendLineRaw($"{types.RenderFunctionReturn(elem.ReturnValue.ParamType)}:");
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