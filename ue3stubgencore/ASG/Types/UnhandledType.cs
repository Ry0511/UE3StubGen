using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

// TODO: Remove this and actually create types for the unhandled types i.e., MapType, DelegateType (though delegate should be handled)
public class UnhandledType(UProperty unhandledType, BaseElement? parent) : BaseType(parent)
{
    public UProperty HeldType { get; } = unhandledType;

    public override string Name()
    {
        return "UnhandledType";
    }
}