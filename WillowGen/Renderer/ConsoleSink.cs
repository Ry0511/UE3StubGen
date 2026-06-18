namespace WillowGen.Renderer;

public class ConsoleSink : ISink
{
    private const int IndentStep = 2;
    private int _indent;

    public void Append(string text)
    {
        Console.Write(new string(' ', _indent) + text);
    }

    public void AppendLine(string text)
    {
        Console.WriteLine(new string(' ', _indent) + text);
    }

    public void PushIndent()
    {
        _indent += IndentStep;
    }

    public void PopIndent()
    {
        _indent -= IndentStep;
    }
}