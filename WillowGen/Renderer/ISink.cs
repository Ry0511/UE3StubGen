namespace WillowGen.Renderer;

public interface ISink
{
    public void Append(string text);
    public void AppendLine(string text = "");
    public void PushIndent();
    public void PopIndent();
}