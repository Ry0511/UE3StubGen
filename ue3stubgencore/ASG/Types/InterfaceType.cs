using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class InterfaceType : BaseType
{
    public NamedType InterfaceClass { get; }

    public InterfaceType(UInterfaceProperty prop, BaseElement? parent)
        : base(parent)
    {
        InterfaceClass = new NamedType(prop.InterfaceClass.GetPath(), this);
    }

    public override IEnumerable<BaseElement> Children()
    {
        yield return InterfaceClass;
    }

    public override string Name()
    {
        return InterfaceClass.Name();
    }
}
