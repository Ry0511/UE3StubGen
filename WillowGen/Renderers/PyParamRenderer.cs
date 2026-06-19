using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem) : IRenderable
{
    // TODO: we need need some type of mechanism to register an import and another to resolve it.
    //  That mechanism must consider cross-package imports and potentially duplicated names i.e.,
    //    from foo import baz
    //    from bar import baz # baz is already in use
    //
    // Probably want two things a top level completely unique reference and a file-scoped-reference
    //

    public void Render(Sink sink)
    {
        if (elem.Parent is ClassDef cls && cls.Name() == "Object")
        {
            sink.Append($"# {elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType)}");
        }
        else
        {
            sink.Append($"{elem.Name()}: {RendererUtils.GetTypeName(elem.ParamType)}");
        }
    }
}