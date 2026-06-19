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
        private readonly FileStream _fs = File.OpenWrite(path);

        protected override void Write(string text)
        {
            Console.Write(text);
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
        var cls = py.Descendants().OfType<ClassDef>()
            .FirstOrDefault(e => e.Name() == "Object")!;

        var sink = new FileSink($"C:\\mod_tools\\decompressed_packages\\BL1\\decompressed\\test.py");

        new PyClassRenderer(cls).Render(sink);
        sink.Dispose();
    }
}