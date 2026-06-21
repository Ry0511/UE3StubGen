using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;
using UE3StubGenCore.Sinks;
using WillowGen.Renderers;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        MultiModuleProject py = new (model);

        var stubDir = @"C:\mod_tools\ue3stubs\bl1";
        {
            var path = stubDir + @"\stubgenapi.pyi";
            var sink = new FileSink(path);
            new PyStubApiRenderer().Render(sink);
            sink.Dispose();
        }

        foreach (var module in py.Modules)
        {
            var path = stubDir + $@"\{module.Name()}\__init__.pyi";
            var sink = new FileSink(path);
            new PyInitFileRenderer(module).Render(sink);
            sink.Dispose();
        }

        foreach (var cls in py.Descendants().OfType<ClassDef>())
        {
            var path = stubDir + $@"\{cls.Module!.Name()}\" + cls.Name() + ".pyi";
            Directory.CreateDirectory(Path.GetDirectoryName(path) !);
            var sink = new FileSink(path);
            new PyClassRenderer(cls).Render(sink);
            sink.Dispose();
        }
    }
}
