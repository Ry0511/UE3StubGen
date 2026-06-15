using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class ClassDef : BaseElement, ISymbol
{
    public ExportClass Export { get; }
    public RefNode? Super { get; }
    public IReadOnlyList<RefNode> Interfaces { get; }
    public IReadOnlyList<TypedParamDef> Fields { get; }
    public IReadOnlyList<FunctionDef> Functions { get; }

    public ClassDef(ExportClass export, BaseElement? parent) : base(parent)
    {
        Export = export;
        if (export.Super != null)
        {
            Super = new RefNode(export.Super, this);
        }

        Interfaces = export.Interfaces.Select(elem => new RefNode(elem, this)).ToList();
        Fields = export.Fields.Select(elem => new TypedParamDef(elem, this)).ToList();
        Functions = export.Functions.Select(elem => new FunctionDef(elem, this)).ToList();
    }

    public override IEnumerable<BaseElement> Children()
    {
        if (Super != null) yield return Super;
        foreach (var elem in Interfaces) yield return elem;
        foreach (var elem in Fields) yield return elem;
        foreach (var elem in Functions) yield return elem;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => true;
}