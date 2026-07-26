using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyDelegateRenderer(TypedParamDef elem, NamingScope scope) : IRenderable
{
    public void Render(Sink sink)
    {
        var dele = (elem.ParamType as DelegateType)!;
        var func = (dele.Function.ResolvedTo as FunctionDef)!;
        new PyFunctionRenderer(func, scope).RenderDelegate(sink, elem);
        sink.AppendLine();
    }
}