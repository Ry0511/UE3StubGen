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

    /**
     * @return the immediate children of this node
     */
    public virtual IEnumerable<PyBaseElement> Children() => [];

    public IEnumerable<PyBaseElement> Descendants()
    {
        foreach (var child in Children())
        {
            yield return child;
            foreach (var descendant in child.Descendants())
            {
                yield return descendant;
            }
        }
    }
}