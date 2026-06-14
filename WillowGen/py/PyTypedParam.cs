using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyTypedParam : PyBaseElement, ISymbol
{
    public ExportField Export { get; }
    public PyRef Type { get; }

    public PyTypedParam(ExportField export, PyBaseElement? parent) : base(parent)
    {
        Export = export;
        Type = new PyRef(export, parent);
    }

    public override IEnumerable<PyBaseElement> Children()
    {
        yield return Type;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => false;
}