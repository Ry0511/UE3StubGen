using System.Text;
using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;
using WillowGen.Renderers;
using WillowGen.Sinks;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    // TODO: the sdk has custom return types for out parameters
    //
    // TODO: WrappedStruct[T] can represent function parameters i.e., all hook functions use
    //  WrappedStruct[T] to capture the parameters. Would want to type-hint the hookable functions
    //  as well if possible without duplicating every functions signature
    //  

    public class FileSink(string path) : Sink(0, DefaultIndentStep)
    {
        private readonly FileStream _fs = new(path, FileMode.Truncate, FileAccess.Write, FileShare.Write);

        protected override void Write(string text)
        {
            _fs.Write(Encoding.UTF8.GetBytes(text));
        }

        public void Dispose()
        {
            _fs.Flush();
            _fs.Dispose();
        }
    }

    public void Export(ExportModel model)
    {
        MultiModuleProject py = new(model);

        var stubDir = @"C:\mod_tools\ue3stubs\bl1";

        {
            var path = stubDir + @"\stubgenapi.pyi";
            File.Create(path).Dispose();
            var sink = new FileSink(path);
            new PyStubApiRenderer().Render(sink);
            sink.Dispose();
        }

        foreach (var module in py.Modules)
        {
            var path = stubDir + $@"\{module.Name()}\__init__.pyi";
            File.Create(path).Dispose();
            var sink = new FileSink(path);
            new PyInitFileRenderer(module).Render(sink);
            sink.Dispose();
        }

        foreach (var cls in py.Descendants().OfType<ClassDef>())
        {
            var path = stubDir + $@"\{cls.Module!.Name()}\" + cls.Name() + ".pyi";
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var sink = new FileSink(path);
            new PyClassRenderer(cls).Render(sink);
            sink.Dispose();
        }
    }
}