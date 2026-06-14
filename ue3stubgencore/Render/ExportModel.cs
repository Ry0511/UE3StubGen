using UE3StubGenCore.Export;

namespace UE3StubGenCore.Render;

public class ExportModel
{
    public ExportContext Context { get; }
    public IReadOnlyList<ExportPackage> Packages { get; private set; }
    public SymbolRegistry Registry { get; private set; }

    public ExportModel(ExportContext ctx)
    {
        Context = ctx;
        Packages = ctx.GetPackages()
            .Select(pkg => new ExportPackage(Context, pkg))
            .ToList();
        Registry = new SymbolRegistry();
    }
}