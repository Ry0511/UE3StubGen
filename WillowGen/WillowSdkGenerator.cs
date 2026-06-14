using UE3StubGenCore.Render;
using WillowGen.py;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        foreach (var pkg in model.Packages)
        {
            PyModule module = new(pkg);
            Console.WriteLine($"Module={module.Name}");
            Console.WriteLine($"  Class Count {module.Classes.Count}");
            Console.WriteLine($"  Struct Count {module.Structures.Count}");
            Console.WriteLine($"  Enum Count {module.Enums.Count}");
            Console.WriteLine("End");
        }
    }
}