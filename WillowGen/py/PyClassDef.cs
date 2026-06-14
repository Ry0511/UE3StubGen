using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyClassDef : PyBaseElement, IPySymbol
{
    public PyRef? Super { get; }
    public IReadOnlyList<PyRef> Interfaces { get; }
    public IReadOnlyList<PyTypedParam> Fields { get; }
    public IReadOnlyList<PyFunctionDef> Functions { get; }

    public PyClassDef(ExportClass export, PyBaseElement? parent) : base(parent)
    {
        if (export.Super != null)
        {
            Super = new PyRef(export.Super, this);
        }

        Interfaces = export.Interfaces.Select(elem => new PyRef(elem, this)).ToList();
        Fields = export.Fields.Select(elem => new PyTypedParam(elem, this)).ToList();
        Functions = export.Functions.Select(elem => new PyFunctionDef(elem, this)).ToList();
    }
}