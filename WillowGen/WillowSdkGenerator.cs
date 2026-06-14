using UE3StubGenCore.Render;
using WillowGen.py;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        PyProject py = new(model);
        foreach (var cls in py.Descendants().OfType<PyClassDef>())
        {
            Console.WriteLine($"Package={cls.Module!.ExportPathName()} Class={cls.ExportPathName()}");
        }
    }
}