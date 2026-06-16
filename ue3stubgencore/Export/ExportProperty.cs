using UELib;
using UELib.Core;
using UELib.Flags;
using UELib.Types;

namespace UE3StubGenCore.Export;

public class ExportProperty : BaseExport
{
    public string Name { get; }
    public PropertyType Type { get; }

    public ExportProperty(ExportContext ctx, UnrealPackage pkg, UProperty obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UProperty>(obj);
        }

        Name = obj.Name;
        Type = obj.Type;
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