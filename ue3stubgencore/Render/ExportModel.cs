using ue3stubgencore.export;

namespace ue3stubgencore.Render;

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