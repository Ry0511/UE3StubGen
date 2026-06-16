using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class UnhandledType(UProperty unhandledType, BaseElement? parent) : BaseType(parent)
{
    public UProperty HeldType { get; } = unhandledType;
}