using UELib;
using UELib.Core;
using UELib.Flags;

namespace UE3StubGenCore.Export;

public class ExportProperty : BaseExport
{
    public ExportProperty(ExportContext _, UnrealPackage pkg, UProperty obj) : base(pkg, obj)
    {
    }

    private UProperty Obj()
    {
        return (ObjectHandle as UProperty)!;
    }

    public bool IsClassMember()
    {
        return Obj().Outer is UClass;
    }

    public bool IsFunctionParameter()
    {
        return Obj().Outer is UFunction;
    }

    public bool IsOutParam()
    {
        return Obj().PropertyFlags.HasFlag(PropertyFlag.OutParm);
    }

    public bool IsOptionalParam()
    {
        return Obj().PropertyFlags.HasFlag(PropertyFlag.OptionalParm);
    }

    public bool IsReturnParam()
    {
        return Obj().PropertyFlags.HasFlag(PropertyFlag.ReturnParm);
    }

    public bool IsStaticArray()
    {
        return Obj().ArrayDim > 1;
    }

    public bool IsDynamicArray()
    {
        return Obj() is UArrayProperty;
    }

    public bool IsArray()
    {
        return IsStaticArray() || IsDynamicArray();
    }
}