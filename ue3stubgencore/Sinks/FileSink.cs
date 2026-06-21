using System.Text;

namespace UE3StubGenCore.Sinks;

public sealed class FileSink(string path) : Sink(0, DefaultIndentStep), IDisposable
{
    private readonly StreamWriter _fs = new(path, append: false, Encoding.UTF8);

    protected override void Write(string text)
    {
        _fs.Write(text);
    }

    public void Dispose()
    {
        _fs.Flush();
        _fs.Dispose();
    }
}