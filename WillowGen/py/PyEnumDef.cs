using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyEnumDef(ExportEnum export, PyBaseElement? parent = null) : PyBaseElement(parent), ISymbol
{
    public ExportEnum Export { get; } = export;
    public IReadOnlyList<string> Values { get; } = export.Ordinals;

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => true;
}