using UELib;
using UELib.Core;
using UELib.Flags;
using UELib.Types;

namespace ue3stubgencore.export;

public class ExportField : BaseExport
{
    public string Name { get; private set; }
    public PropertyType Type { get; private set; }
    public bool IsOutParm { get; private set; }
    public bool IsOptionalParm { get; private set; }
    public bool IsReturnParm { get; private set; }
    public bool IsClassMember { get; private set; }
    public bool IsFunctionParameter { get; private set; }

    public ExportField(ExportContext ctx, UnrealPackage pkg, UProperty obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UProperty>(pkg, obj);
        }

        Name = obj.Name;
        Type = obj.Type;

        // `optional out` is valid
        IsOutParm = obj.PropertyFlags.HasFlag(PropertyFlag.OutParm);
        IsOptionalParm = obj.PropertyFlags.HasFlag(PropertyFlag.OptionalParm);
        IsReturnParm = obj.PropertyFlags.HasFlag(PropertyFlag.ReturnParm);
        IsClassMember = obj.Outer is UClass;
        IsFunctionParameter = obj.Outer is UFunction;
    }
}