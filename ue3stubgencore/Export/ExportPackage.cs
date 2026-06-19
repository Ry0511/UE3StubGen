using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportPackage : BaseExport
{
    public IReadOnlyList<ExportClass> Classes { get; }

    public ExportPackage(ExportContext ctx, UnrealPackage pkg) : base(pkg, pkg.RootPackage)
    {
        Classes = pkg.Objects.OfType<UClass>()
            .Where(IsExport)
            .Select(cls => ctx.CreateExport<ExportClass>(pkg, cls))
            .ToList();
    }
}