using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportEnum : BaseExport
{
    public List<string> Ordinals { get; } = [];

    public ExportEnum(ExportContext ctx, UnrealPackage pkg, UEnum obj)
        : base(pkg, obj)
    {
        if (IsImport(obj))
            obj = ctx.ResolveImport<UEnum>(obj);

        // the ordinal value is implied from index
        foreach (var name in obj.Names)
            Ordinals.Add(name.ToString());
    }
}
