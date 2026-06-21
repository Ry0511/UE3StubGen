namespace WillowGen.Sinks;

public class ConsoleSink(int indent = 0, int step = Sink.DefaultIndentStep) : Sink(indent, step)
{
    public ConsoleSink(Sink parent)
        : this(parent.IndentLevel, parent.IndentStep) { }

    protected override void Write(string text)
    {
        Console.Write(text);
    }
}
