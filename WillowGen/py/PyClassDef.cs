using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyClassDef : PyBaseElement, IPyExportSymbol
{
    public string Name { get; }
    public PyRef? Super { get; }
    public IReadOnlyList<PyRef> Interfaces { get; }
    public IReadOnlyList<PyTypedParam> Fields { get; }
    public IReadOnlyList<PyFunctionDef> Functions { get; }

    public PyClassDef(ExportClass export, PyBaseElement? parent) : base(parent)
    {
        Name = export.Name;
        if (export.Super != null)
        {
            Super = new PyRef(export.Super, this);
        }

        Interfaces = export.Interfaces.Select(elem => new PyRef(elem, this)).ToList();
        Fields = export.Fields.Select(elem => new PyTypedParam(elem, this)).ToList();
        Functions = export.Functions.Select(elem => new PyFunctionDef(elem, this)).ToList();
    }

    public override IEnumerable<PyBaseElement> Children()
    {
        if (Super != null) yield return Super;
        foreach (var elem in Interfaces) yield return elem;
        foreach (var elem in Fields) yield return elem;
        foreach (var elem in Functions) yield return elem;
    }
}