using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportStruct : BaseExport
{
    public ExportStruct? Super { get; private set; }
    public List<ExportField> Fields { get; } = [];

    public ExportStruct(ExportContext ctx, UnrealPackage pkg, UStruct obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            // UStruct/UScriptStruct
            obj = ctx.ResolveImport<UStruct>(obj, checkSubclasses: true);
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