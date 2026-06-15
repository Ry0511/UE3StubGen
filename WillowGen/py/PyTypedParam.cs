using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyTypedParam(ExportField export, PyBaseElement? parent) : PyBaseElement(parent), ISymbol
{
    public ExportField Export { get; } = export;
    public PyRef Type { get; } = new(export, export.TargetTypeFullPath, parent);

    public override IEnumerable<PyBaseElement> Children()
    {
        yield return Type;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => false;
}