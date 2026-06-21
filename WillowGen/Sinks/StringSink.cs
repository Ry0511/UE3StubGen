using System.Text;

namespace WillowGen.Sinks;

public class StringSink(int level = 0, int step = Sink.DefaultIndentStep) : Sink(level, step)
{
    private readonly StringBuilder _sb = new();

    public StringSink(Sink parent)
        : this(parent.IndentLevel, parent.IndentStep) { }

    protected override void Write(string text)
    {
        _sb.Append(text);
    }

    public override string ToString()
    {
        return _sb.ToString();
    }

    public void Clear()
    {
        _sb.Clear();
    }

    public override void Reset(Sink other)
    {
        base.Reset(other);
        Clear();
    }
}
