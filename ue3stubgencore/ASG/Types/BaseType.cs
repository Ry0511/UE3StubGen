using UE3StubGenCore.Export;
using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public abstract class BaseType(BaseElement? parent) : BaseElement(parent)
{
    public static BaseType Create(ExportProperty elem, BaseElement? parent)
    {
        return Create((elem.ObjectHandle as UProperty)!, parent);
    }

    protected static BaseType Create(UProperty prop, BaseElement? parent, bool ignoreArrayDim = false)
    {
        if (prop.ArrayDim > 1 && !ignoreArrayDim)
        {
            return new StaticArrayType(prop, parent);
        }

        if (prop is UArrayProperty arrayProp)
        {
            return new DynArrayType(arrayProp, parent);
        }

        if (prop is UClassProperty cls)
        {
            return new ClassType(cls, parent);
        }

        if (prop is UMapProperty or UDelegateProperty)
        {
            return new UnhandledType(prop, parent);
        }

        if (prop is UByteProperty { Enum: not null } byteProp)
        {
            return new NamedType(byteProp.Enum.GetPath(), parent);
        }

        if (prop is UInterfaceProperty iface)
        {
            return new InterfaceType(iface, parent);
        }

        if (prop is UObjectProperty or UComponentProperty)
        {
            return new NamedType((prop as UObjectProperty)!.Object.GetPath(), parent);
        }

        if (prop is UStructProperty structProp)
        {
            return new NamedType(structProp.Struct.GetPath(), parent);
        }

        try
        {
            return new EngineBuiltinType(prop, parent);
        }
        catch (Exception err)
        {
            Console.WriteLine($"Unhandled property type: {prop.GetType().Name} {err.Message}");
            return new UnhandledType(prop, parent);
        }
    }
}