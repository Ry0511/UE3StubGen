using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class FunctionDef : BaseElement, ISymbol, INameable
{
    public ExportFunction Export { get; }
    public List<TypedParamDef> Params { get; }
    public TypedParamDef? ReturnValue { get; }

    public FunctionDef(ExportFunction export, BaseElement? parent = null) : base(parent)
    {
        if (!export.IsRegularFunction)
        {
            throw new Exception("only regular functions are supported (iterator and operator functions are not allowed)");
        }

        Export = export;
        Params = export.Parameters.Select(elem => new TypedParamDef(elem, this)).ToList();
        ReturnValue = export.ReturnParameter != null ? new TypedParamDef(export.ReturnParameter, this) : null;
    }

    public override IEnumerable<BaseElement> Children()
    {
        foreach (var elem in Params) yield return elem;
        if (ReturnValue != null) yield return ReturnValue;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public string Name() => Export.Name();
}