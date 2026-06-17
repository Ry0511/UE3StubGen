using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class TypedParamDef : BaseElement, ISymbol
{
    public string ParamName { get; }
    public BaseType ParamType { get; }

    public TypedParamDef(ExportProperty prop, BaseElement? parent = null) : base(parent)
    {
        ParamName = prop.Name();
        ParamType = BaseType.Create(prop, this);
    }

    public string ExportPathName() => ParamName;
    public bool CanBeReferenced() => false;
}