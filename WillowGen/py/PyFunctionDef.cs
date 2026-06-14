using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyFunctionDef : PyBaseElement, ISymbol
{
    public ExportFunction Export { get; }
    public List<PyTypedParam> Params { get; }
    public PyTypedParam? ReturnValue { get; }

    public PyFunctionDef(ExportFunction export, PyBaseElement? parent = null) : base(parent)
    {
        Export = export;
        Params = export.Parameters.Select(elem => new PyTypedParam(elem, this)).ToList();
        ReturnValue = export.ReturnParameter != null ? new PyTypedParam(export.ReturnParameter, this) : null;
    }

    public override IEnumerable<PyBaseElement> Children()
    {
        foreach (var elem in Params) yield return elem;
        if (ReturnValue != null) yield return ReturnValue;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => false;
}