using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class DynArrayType : BaseType
{
    public BaseType InnerType { get; }

    public DynArrayType(UArrayProperty prop, BaseElement? parent = null) : base(parent)
    {
        InnerType = Create(prop.InnerProperty, this);
    }
    
    public override IEnumerable<BaseElement> Children()
    {
        yield return InnerType;
    }
}