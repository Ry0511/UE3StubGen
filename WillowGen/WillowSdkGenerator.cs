using UE3StubGenCore.Render;
using WillowGen.py;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        PyProject py = new(model);

        foreach (var node in py.Descendants())
        {
            Console.WriteLine(node.GetType().Name);
        }
    }
}