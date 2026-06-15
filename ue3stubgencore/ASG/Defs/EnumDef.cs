using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class EnumDef(ExportEnum export, BaseElement? parent = null) : BaseElement(parent), ISymbol
{
    public ExportEnum Export { get; } = export;
    public IReadOnlyList<string> Values { get; } = export.Ordinals;

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => true;
}