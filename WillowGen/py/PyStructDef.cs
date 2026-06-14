using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyStructDef : PyBaseElement, IPySymbol
{
    private ExportStruct _export;

    public PyRef? Super { get; }
    public IReadOnlyList<PyTypedParam> Fields { get; }

    public PyStructDef(ExportStruct export, PyBaseElement? parent = null) : base(parent)
    {
        _export = export;

        if (export.Super != null)
        {
            Super = new PyRef(export.Super, this);
        }

        Fields = export.Fields.Select(elem => new PyTypedParam(elem, this)).ToList();
    }
}