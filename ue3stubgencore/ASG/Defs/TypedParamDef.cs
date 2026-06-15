using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class TypedParamDef(ExportField export, BaseElement? parent) : BaseElement(parent), ISymbol
{
    public ExportField Export { get; } = export;
    public RefNode Type { get; } = new(export, export.TargetTypeFullPath, parent);

    public override IEnumerable<BaseElement> Children()
    {
        yield return Type;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => false;
}