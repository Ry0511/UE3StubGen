using UE3StubGenCore.Export;

namespace UE3StubGenCore.Render;

public class ExportModel
{
    public DirectoryInfo OutputDirectory { get; }

    public string ImportRoot { get; }

    public ExportContext Context { get; }

    public IReadOnlyList<ExportPackage> Packages { get; private set; }

    public ExportModel(string outputDirectory, string importRoot, ExportContext ctx)
    {
        OutputDirectory = new DirectoryInfo(outputDirectory);
        if (!OutputDirectory.Exists)
        {
            throw new Exception($"output directory does not exist: {outputDirectory}");
        }

        ImportRoot = importRoot;

        Context = ctx;
        Packages = ctx.GetPackages().Select(pkg => new ExportPackage(Context, pkg)).ToList();
    }

    public void ExportAll(IExporter api)
    {
        api.Export(this);
    }
}