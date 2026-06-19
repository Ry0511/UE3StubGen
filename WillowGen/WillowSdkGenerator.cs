using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;
using WillowGen.Renderers;
using WillowGen.Sinks;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        MultiModuleProject py = new(model);
        var cls = py.Descendants().OfType<ClassDef>()
            .FirstOrDefault(e => e.Name() == "GFxMovie")!;

        new PyClassRenderer(cls).Render(new ConsoleSink());
    }
}