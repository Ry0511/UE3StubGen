using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public enum EngineBuiltin
{
    Integer,
    Float,
    Bool,
    Byte,
    Name,
    String
}

public class EngineBuiltinType(UProperty prop, BaseElement? parent = null) : BaseType(parent)
{
    public EngineBuiltin Type { get; } = prop switch
    {
        UIntProperty => EngineBuiltin.Integer,
        UFloatProperty => EngineBuiltin.Float,
        UBoolProperty => EngineBuiltin.Bool,
        UByteProperty => EngineBuiltin.Byte,
        UNameProperty => EngineBuiltin.Name,
        UStrProperty => EngineBuiltin.String,
        _ => throw new Exception("property is not a valid primitive type: " + prop.GetType())
    };

    public override string Name() => Type.ToString();
}