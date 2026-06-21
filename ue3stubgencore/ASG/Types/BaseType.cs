using UE3StubGenCore.Export;
using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public abstract class BaseType(BaseElement? parent) : BaseElement(parent), INameable
{
    public static BaseType Create(ExportProperty elem, BaseElement? parent)
    {
        return Create((elem.ObjectHandle as UProperty) !, parent);
    }

    protected static BaseType Create(
        UProperty prop,
        BaseElement? parent,
        bool ignoreArrayDim = false)
    {
        if (prop.ArrayDim > 1 && !ignoreArrayDim)
        {
            return new StaticArrayType(prop, parent);
        }

        switch (prop)
        {
            case UArrayProperty e:
                return new DynArrayType(e, parent);
            case UClassProperty e:
                return new ClassType(e, parent);
            case UMapProperty e:
                return new UnhandledType(e, parent);
            case UDelegateProperty e:
                return new DelegateType(e, parent);
            case UByteProperty e:
            {
                if (e.Enum != null)
                    {
                        return new NamedType(e.Enum.GetPath(), parent);
                    }

                return new EngineBuiltinType(e, parent);
            }

            case UInterfaceProperty e:
                return new InterfaceType(e, parent);
            case UComponentProperty e:
                return new NamedType(e.Object.GetPath(), parent);
            case UStructProperty e:
                return new NamedType(e.Struct.GetPath(), parent);
            case UObjectProperty e:
                return new NamedType(e.Object.GetPath(), parent);

            default:
            {
                try
                {
                    return new EngineBuiltinType(prop, parent);
                }
                catch (Exception)
                {
                    return new UnhandledType(prop, parent);
                }
            }
        }
    }

    public abstract string Name();
}
