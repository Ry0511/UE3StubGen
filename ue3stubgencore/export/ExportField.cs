using UELib;
using UELib.Core;
using UELib.Flags;

namespace ue3stubgencore.export;

/**
 * represents a class member field, struct member field, and a function parameter
 */
public class ExportField : BaseExport
{
    public string Name { get; private set; }
    public string TypeName { get; private set; }
    public bool IsOutParm { get; private set; }
    public bool IsOptionalParm { get; private set; }
    public bool IsReturnParm { get; private set; }
    public bool IsClassMember { get; private set; }
    public bool IsFunctionMember { get; private set; }

    public ExportField(ExportContext ctx, UnrealPackage pkg, UProperty obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UProperty>(pkg, obj)!;
        }

        Name = obj.Name;
        TypeName = obj switch
        {
            UObjectProperty e => e.GetFriendlyType(), // @NULL or class name
            UStructProperty e => e.Struct.Name,
            _ => obj.Type.ToString()
        };
        // `optional out` is valid
        IsOutParm = obj.PropertyFlags.HasFlag(PropertyFlag.OutParm);
        IsOptionalParm = obj.PropertyFlags.HasFlag(PropertyFlag.OptionalParm);
        IsReturnParm = obj.PropertyFlags.HasFlag(PropertyFlag.ReturnParm);
        IsClassMember = obj.Outer is UClass;
        IsFunctionMember = obj.Outer is UFunction;
    }
}