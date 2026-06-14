namespace WillowGen.py;

public abstract class PyBaseElement(PyBaseElement? parent = null)
{
    public PyBaseElement? Parent { get; } = parent;

    public PyBaseElement? Outermost()
    {
        var node = Parent;
        while (node != null && node.Parent != null)
        {
            node = node.Parent;
        }

        return node;
    }
}