using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;
using WillowGen.Renderers.cls;

namespace WillowGen.Renderers;

public class PyArgumentListRenderer(string importRoot, ClassDef elem) : IRenderable
{
    private readonly PyImportRenderer _importRenderer = new(importRoot, elem);

    public void Render(Sink sink)
    {
        foreach (var func in elem.Functions.Where(e => e.Params.Count > 0))
        {
            sink.AppendLine($"class {elem.Name()}{func.Name()}(WrappedStruct):");
            sink.PushIndent();

            foreach (var param in func.Params)
            {
                new PyParamRenderer(param, _importRenderer.NameResolver).Render(sink);
                sink.AppendLine();
            }

            sink.PopIndent();
            sink.AppendLine();
        }
    }
}