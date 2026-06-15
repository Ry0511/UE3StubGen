using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG;

public class RefNode : BaseElement
{
    /**
     * The export object that this reference was constructed from.
     */
    public BaseExport Export { get; }

    /**
     * The path to the target object of which this reference is referencing.
     */
    public string TargetFullPath { get; }

    /**
     * The node element that this reference resolves to.
     */
    public BaseElement? ResolvedTo { get; set; } = null;

    /**
     * Constructs a new reference to the given export object.
     */
    public RefNode(BaseExport export, BaseElement? parent)
        : this(export, export.ObjectHandle.GetPath(), parent)
    {
    }

    /**
     * creates a reference to the given target path
     */
    public RefNode(BaseExport export, string targetPath, BaseElement? parent) : base(parent)
    {
        Export = export;
        TargetFullPath = targetPath;
    }
}