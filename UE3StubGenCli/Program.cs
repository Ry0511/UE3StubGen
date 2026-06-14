using UE3StubGenCore.Export;
using UE3StubGenCore.Render;
using WillowGen;

namespace UE3StubGenCli;

internal class LoggingExporter : IExporter
{
    public void Export(ExportModel model)
    {
        foreach (var pkg in model.Packages)
        {
            Console.WriteLine($"Package {pkg.PackageName} has {pkg.Classes.Count} classes");
        }
    }

    public static void Main(string[] args)
    {
        string root = @"C:\mod_tools\decompressed_packages\BL1\decompressed";
        ExportModel model = new ExportModel(new ExportContext(root));
        model.ExportAll(new LoggingExporter());
        model.ExportAll(new WillowSdkGenerator());
        Console.WriteLine("Cache Size = " + model.Context.CacheCount);
    }
}