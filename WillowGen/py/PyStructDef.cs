using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyStructDef : PyBaseElement, ISymbol
{
    public ExportStruct Export { get; }
    public PyRef? Super { get; }
    public IReadOnlyList<PyTypedParam> Fields { get; }

    public PyStructDef(ExportStruct export, PyBaseElement? parent = null) : base(parent)
    {
        Export = export;

        if (export.Super != null)
        {
            Super = new PyRef(export.Super, this);
        }

        Fields = export.Fields.Select(elem => new PyTypedParam(elem, this)).ToList();
    }

    public override IEnumerable<PyBaseElement> Children()
    {
        if (Super != null) yield return Super;
        foreach (var elem in Fields) yield return elem;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => true;
}