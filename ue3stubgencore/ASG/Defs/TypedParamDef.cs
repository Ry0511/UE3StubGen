using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class TypedParamDef : BaseElement, INameable
{
    public string ParamName { get; }
    public BaseType ParamType { get; }
    public bool IsFunctionParam { get; }
    public bool IsOptionalParam { get; }
    public bool IsOutParam { get; }

    public TypedParamDef(ExportProperty prop, BaseElement? parent = null)
        : base(parent)
    {
        ParamName = prop.Name();
        ParamType = BaseType.Create(prop, this);
        IsFunctionParam = parent is FunctionDef;
        IsOptionalParam = prop.IsOptionalParam();
        IsOutParam = prop.IsOutParam();
    }

    public string Name()
    {
        return ParamName;
    }

    public override IEnumerable<BaseElement> Children()
    {
        yield return ParamType;
    }
}
