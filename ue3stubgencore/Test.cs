using UE3StubGenCore.Export;
using UE3StubGenCore.Render;

namespace UE3StubGenCore;

public static class UClassInspector
{
    class LoggingExporter : IExporter
    {
        public void Export(ExportModel model, ExportPackage pkg)
        {
            Console.WriteLine($"{pkg.PackageName} has {pkg.Classes.Count} classes");
        }
    }

    public static void Hello()
    {
        string root = @"C:\mod_tools\decompressed_packages\BL1\decompressed";
        ExportModel model = new ExportModel(new ExportContext(root));
        model.ExportAll(new LoggingExporter());
        Console.WriteLine("Cache Size = " + model.Context.CacheCount);
    }
}