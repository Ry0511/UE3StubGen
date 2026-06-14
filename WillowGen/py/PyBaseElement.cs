namespace WillowGen.py;

public abstract class PyBaseElement(PyBaseElement? parent = null)
{
    public PyBaseElement? Parent { get; protected set; } = parent;
    public virtual IEnumerable<PyBaseElement> Children() => [];
}