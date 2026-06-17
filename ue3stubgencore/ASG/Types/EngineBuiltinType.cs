using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class EngineBuiltinType : BaseType
{
    public string FriendlyName { get; }

    public EngineBuiltinType(UProperty prop, BaseElement? parent = null) : base(parent)
    {
        FriendlyName = prop switch
        {
            UIntProperty
                or UFloatProperty
                or UBoolProperty
                or UByteProperty
                or UNameProperty
                or UStrProperty => prop.GetFriendlyType(),
            _ => throw new Exception("property is not a valid primitive type: " + prop.GetType())
        };
    }
}