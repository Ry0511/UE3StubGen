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
        MultiModuleProject py = new(model);

        var stubDir = model.OutputDirectory.FullName;
        {
            var path = stubDir + @"\stubgenapi.pyi";
            Console.WriteLine($"creating {path}");
            var sink = new FileSink(path);
            new PyStubApiRenderer().Render(sink);
            sink.Dispose();
        }

        // create all the __init__.pyi files for each module, these will export the unique module
        // names to the module so they can be accessed directly from there via
        // from Module import Class|Enum|Struct|Delegate
        foreach (var module in py.Modules)
        {
            var path = stubDir + $@"\{module.Name()}\__init__.pyi";
            Console.WriteLine($"creating {path}");
            var sink = new FileSink(path);
            new PyInitFileRenderer(module).Render(sink);
            sink.Dispose();
        }

        // create all the class.pyi files (in their package directory)
        foreach (var cls in py.Descendants().OfType<ClassDef>())
        {
            var path = stubDir + $@"\{cls.Module!.Name()}\" + cls.Name() + ".pyi";
            Console.WriteLine($"creating {path}");
            Directory.CreateDirectory(Path.GetDirectoryName(path) !);
            var sink = new FileSink(path);
            new PyClassRenderer(model.ImportRoot, cls).Render(sink);
            sink.Dispose();
        }
    }
}