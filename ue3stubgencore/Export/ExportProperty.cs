using UELib;
using UELib.Core;
using UELib.Flags;

namespace UE3StubGenCore.Export;

public class ExportProperty : BaseExport
{
    public ExportProperty(ExportContext _, UnrealPackage pkg, UProperty obj) : base(pkg, obj)
    {
    }

    private UProperty Obj() => (ObjectHandle as UProperty)!;

    public bool IsClassMember() => Obj().Outer is UClass;

    public bool IsFunctionParameter() => Obj().Outer is UFunction;
    public bool IsOutParam() => Obj().PropertyFlags.HasFlag(PropertyFlag.OutParm);
    public bool IsOptionalParam() => Obj().PropertyFlags.HasFlag(PropertyFlag.OptionalParm);
    public bool IsReturnParam() => Obj().PropertyFlags.HasFlag(PropertyFlag.ReturnParm);

    public bool IsStaticArray() => Obj().ArrayDim > 1;
    public bool IsDynamicArray() => Obj() is UArrayProperty;
    public bool IsArray() => IsStaticArray() || IsDynamicArray();
}