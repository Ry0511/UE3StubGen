namespace WillowGen.py;

public abstract class PyBaseElement
{
    public PyBaseElement? Parent { get; }
    public PyModule? Module { get; }

    protected PyBaseElement(PyBaseElement? parent = null)
    {
        Parent = parent;
        Module = Ancestors().OfType<PyModule>().FirstOrDefault();
    }
    
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

    public IEnumerable<PyBaseElement> Ancestors()
    {
        var node = Parent;
        while (node != null)
        {
            yield return node;
            node = node.Parent;
        }
    }
    
}