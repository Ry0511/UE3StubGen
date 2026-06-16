using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class EngineBuiltinType : BaseType
{
    public string FriendlyName { get; }

    public EngineBuiltinType(UProperty prop, BaseElement? parent = null) : base(parent)
    {
        switch (prop)
        {
            case UIntProperty:
            case UFloatProperty:
            case UBoolProperty:
            case UByteProperty:
            case UNameProperty:
            case UStrProperty:
                FriendlyName = prop.GetFriendlyType();
                break;
            default:
                throw new Exception("property is not a valid primitive type: " + prop.GetType());
        }
    }
}