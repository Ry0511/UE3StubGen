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

    public IEnumerable<BaseElement> Descendants(bool includeSelf = false)
    {
        if (includeSelf) yield return this;
        foreach (var child in Children())
        {
            yield return child;
            foreach (var descendant in child.Descendants()) yield return descendant;
        }
    }

    public IEnumerable<BaseElement> Ancestors(bool includeSelf = false)
    {
        if (includeSelf) yield return this;
        var node = Parent;
        while (node != null)
        {
            yield return node;
            node = node.Parent;
        }
    }

    public string BuildName() => string.Join(".",
        Ancestors(includeSelf: true)
            .OfType<INameable>()
            .Select(e => e.Name())
            .Reverse()
    );
}