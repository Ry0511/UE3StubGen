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
            obj = ctx.ResolveImport<UScriptStruct>(obj);
        }

        if (obj.Super != null)
        {
            Super = ctx.CreateExport<ExportStruct>(pkg, obj.Super);
        }

        // TODO: Structs can contain structs see MaterialInstance
        //  .BeastMaterialInstancePropertiesContainer.BeastMaterialInstancePropertiesOverrides
        
        foreach (var elem in obj.EnumerateFields<UProperty>())
        {
            Fields.Add(new ExportField(ctx, pkg, elem));
        }
    }
}