namespace UE3StubGenCore.ASG;

/**
 * A node that can be resolved to another node in the tree by way of an absolute path/name
 */
public class RefNode(string targetFullPath, BaseElement? parent = null) : BaseElement(parent)
{
    /**
     * The path to the target object of which this reference is referencing.
     */
    public string TargetFullPath { get; } = targetFullPath;

    /**
     * The node element that this reference resolves to.
     */
    public BaseSymbol? ResolvedTo { get; set; }
}
