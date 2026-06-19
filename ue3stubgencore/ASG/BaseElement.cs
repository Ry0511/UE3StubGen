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

    public virtual IEnumerable<BaseElement> Children()
    {
        return [];
    }

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

    public string BuildName()
    {
        return string.Join(".",
            Ancestors(true)
                .OfType<INameable>()
                .Select(e => e.Name())
                .Reverse()
        );
    }

    public IEnumerable<PackageDef> AllModules()
    {
        if (Module == null) yield break;
        if (Module.Parent == null)
        {
            yield return Module;
            yield break;
        }

        foreach (var module in Module.Parent.Children().OfType<PackageDef>())
        {
            yield return module;
        }
    }
}