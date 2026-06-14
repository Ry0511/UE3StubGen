using UE3StubGenCore.Export;

namespace UE3StubGenCore.Render;

public class ExportModel
{
    public IReadOnlyList<ExportPackage> Packages { get; private set; }
    public SymbolRegistry Registry { get; private set; }

    public ExportModel(ExportContext ctx)
    {
        Packages = ctx.GetPackages()
            .Select(pkg => new ExportPackage(ctx, pkg))
            .ToList();
        Registry = new SymbolRegistry();
    }
}