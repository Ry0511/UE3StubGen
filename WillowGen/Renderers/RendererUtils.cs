using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public static class RendererUtils
{
    public static IRenderable Create(BaseElement elem)
    {
        return elem switch
        {
            ClassDef e => new PyClassRenderer(e),
            StructDef e => new PyStructRenderer(e),
            EnumDef e => new PyEnumRenderer(e),
            FunctionDef e => new PyFunctionRenderer(e),
            TypedParamDef e => new PyParamRenderer(e),
            _ => throw new Exception("unsupported element type: " + elem.GetType().Name + "")
        };
    }
}