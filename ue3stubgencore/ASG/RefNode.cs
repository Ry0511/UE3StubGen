namespace UE3StubGenCore.ASG;

public class RefNode(string targetFullPath, BaseElement? parent = null) : BaseElement(parent)
{
    /**
     * The path to the target object of which this reference is referencing.
     */
    public string TargetFullPath { get; } = targetFullPath;

    /**
     * The node element that this reference resolves to.
     */
    public BaseElement? ResolvedTo { get; set; }
}