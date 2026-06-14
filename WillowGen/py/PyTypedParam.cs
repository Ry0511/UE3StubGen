using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyTypedParam : PyBaseElement
{
    private ExportField _export;
    public PyRef Type { get; }

    public PyTypedParam(ExportField export, PyBaseElement? parent) : base(parent)
    {
        _export = export;
        Type = new PyRef(export, this);
    }

    public override IEnumerable<PyBaseElement> Children()
    {
        yield return Type;
    }
}