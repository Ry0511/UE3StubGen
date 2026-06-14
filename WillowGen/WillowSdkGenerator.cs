using UE3StubGenCore.Render;
using WillowGen.py;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        PyProject py = new(model);

        Console.WriteLine($"Modules={py.Modules.Count}");
        foreach (var module in py.Modules)
        {
            Console.WriteLine($"  module={module.Name}");
            Console.WriteLine($"    Class Count {module.Classes.Count}");
            Console.WriteLine($"    Struct Count {module.Structures.Count}");
            Console.WriteLine($"    Enum Count {module.Enums.Count}");
            Console.WriteLine("  End");
        }

        Console.WriteLine("End");
    }
}