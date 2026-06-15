using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyRef : PyBaseElement
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
    public PyBaseElement? ResolvedTo { get; set; } = null;

    /**
     * Constructs a new reference to the given export object.
     */
    public PyRef(BaseExport export, PyBaseElement? parent)
        : this(export, export.ObjectHandle.GetPath(), parent)
    {
    }

    /**
     * creates a reference to the given target path
     */
    public PyRef(BaseExport export, string targetPath, PyBaseElement? parent) : base(parent)
    {
        Export = export;
        TargetFullPath = targetPath;
    }
}