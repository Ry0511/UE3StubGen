using System.Text;
using ue3stubgencore.export;
using ue3stubgencore.Render;
using UELib;
using UELib.Core;

namespace ue3stubgencore;

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
    }
}