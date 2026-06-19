using System.Text;
using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;
using WillowGen.Renderers;
using WillowGen.Sinks;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
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