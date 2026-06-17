using UE3StubGenCore.ASG.Defs;

namespace UE3StubGenCore.ASG;

public abstract class BaseElement
{
    public BaseElement? Parent { get; }
    public PackageDef? Module { get; }

    protected BaseElement(BaseElement? parent = null)
    {
        Parent = parent;
        Module = Ancestors().OfType<PackageDef>().FirstOrDefault();
    }

    public virtual IEnumerable<BaseElement> Children() => [];

    public IEnumerable<BaseElement> Descendants()
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

    public IEnumerable<BaseElement> Ancestors()
    {
        var node = Parent;
        while (node != null)
        {
            yield return node;
            node = node.Parent;
        }
    }

    public IEnumerable<BaseElement> WalkUp()
    {
        yield return this;
        foreach (var elem in Ancestors())
        {
            yield return elem;
        }
    }

    public string BuildName() => string.Join(".",
        WalkUp().OfType<INameable>().Select(e => e.Name()).Reverse()
    );
}