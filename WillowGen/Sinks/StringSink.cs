using System.Text;

namespace WillowGen.Sinks;

public class StringSink(int level = 0, int step = Sink.DefaultIndentStep) : Sink(level, step)
{
    private readonly StringBuilder sb = new ();

    public StringSink(Sink parent)
        : this(parent.IndentLevel, parent.IndentStep)
    {
    }

    protected override void Write(string text)
    {
        sb.Append(text);
    }

    public override string ToString()
    {
        return sb.ToString();
    }

    public void Clear()
    {
        sb.Clear();
    }

    public override void Reset(Sink other)
    {
        base.Reset(other);
        Clear();
    }
}
