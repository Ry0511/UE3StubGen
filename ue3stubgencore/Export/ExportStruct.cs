using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportStruct : BaseExport
{
    public ExportStruct? Super { get; }

    public IReadOnlyList<ExportProperty> Fields { get; } = [];

    public IReadOnlyList<ExportStruct> ChildStructs { get; }

    public ExportStruct(ExportContext ctx, UnrealPackage pkg, UStruct obj)
        : base(pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UScriptStruct>(obj);
        }

        if (obj.Super != null)
        {
            Super = ctx.CreateExport<ExportStruct>(pkg, obj.Super);
        }

        Fields = obj.EnumerateFields<UProperty>()
            .Select(e => new ExportProperty(ctx, pkg, e))
            .ToList();

        ChildStructs = obj.EnumerateFields<UScriptStruct>()
            .Select(e => ctx.CreateExport<ExportStruct>(pkg, e))
            .ToList();
    }
}
