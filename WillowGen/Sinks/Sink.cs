namespace WillowGen.Sinks;

public abstract class Sink(int indent, int step)
{
    public const int DefaultIndentStep = 4;
    public int IndentLevel { get; private set; } = indent;
    public int IndentStep { get; private set; } = step;

    protected Sink(Sink other)
        : this(other.IndentLevel, other.IndentStep) { }

    public void AppendRaw(string text)
    {
        Write(text);
    }

    public void AppendLineRaw(string text)
    {
        Write(text + "\n");
    }

    public void Append(string text)
    {
        Write(Indent() + text);
    }

    public void AppendLine(string text = "")
    {
        if (text.Length == 0)
            Write("\n");
        else
            Write(Indent() + text + "\n");
    }

    public void PushIndent()
    {
        IndentLevel += IndentStep;
    }

    public void PopIndent()
    {
        IndentLevel -= IndentStep;
    }

    public void ClearIndent()
    {
        IndentLevel = 0;
    }

    public virtual void Reset(Sink other)
    {
        IndentLevel = other.IndentLevel;
        IndentStep = other.IndentStep;
    }

    private string Indent()
    {
        return new string(' ', IndentLevel);
    }

    protected abstract void Write(string text);
}
