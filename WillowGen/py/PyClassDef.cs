using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyClassDef : PyBaseElement
{
    public IReadOnlyList<PyTypedParam> Fields { get; }
    public IReadOnlyList<PyFunctionDef> Functions { get; }

    public PyClassDef(ExportClass export, PyBaseElement? parent) : base(parent)
    {
        Fields = export.Fields.Select(elem => new PyTypedParam(elem, this)).ToList();
        Functions = export.Functions.Select(elem => new PyFunctionDef(elem, this)).ToList();
    }

    public override IEnumerable<PyBaseElement> Children()
    {
        return Fields.Concat<PyBaseElement>(Functions);
    }
}