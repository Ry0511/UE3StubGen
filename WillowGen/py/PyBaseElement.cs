namespace WillowGen.py;

// TODO: I have used the Py naming convention here but in reality this actually builds a pseudo AST
//  that can be used to generate Python code - But it itself doesn't really generate Python code.
//  with that in mind we can move all of this into StubGenCore.

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