using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class StructDef : BaseElement, ISymbol
{
    public ExportStruct Export { get; }
    public RefNode? Super { get; }
    public IReadOnlyList<TypedParamDef> Fields { get; }

    public StructDef(ExportStruct export, BaseElement? parent = null) : base(parent)
    {
        Export = export;

        if (export.Super != null)
        {
            Super = new RefNode(export.Super, this);
        }

        Fields = export.Fields.Select(elem => new TypedParamDef(elem, this)).ToList();
    }

    public override IEnumerable<BaseElement> Children()
    {
        if (Super != null) yield return Super;
        foreach (var elem in Fields) yield return elem;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => true;
}