using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

public class ExportStruct : BaseExport
{
    public ExportStruct? Super { get; private set; }
    public List<ExportField> Fields { get; } = [];

    public ExportStruct(ExportContext ctx, UnrealPackage pkg, UStruct obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UStruct>(pkg, obj)!;
        }

        if (obj.Super != null)
        {
            Super = new ExportStruct(ctx, pkg, obj.Super);
        }

        foreach (var elem in obj.EnumerateFields<UProperty>())
        {
            Fields.Add(new(ctx, pkg, elem));
        }
    }
}