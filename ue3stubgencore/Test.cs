using UE3StubGenCore.Export;
using UE3StubGenCore.Render;

namespace UE3StubGenCore;

public static class UClassInspector
{
    public static void Hello()
    {
        string root = @"C:\mod_tools\decompressed_packages\BL1\decompressed";
        ExportModel model = new ExportModel(new ExportContext(root));

        foreach (var pkg in model.Packages)
        {
            Console.WriteLine($"{pkg.PackageName} has {pkg.Classes.Count} classes");
        }
        
        Console.WriteLine("Cache Size = " + model.Context.CacheCount);
    }
}