using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class PackageDef : BaseElement, ISymbol, INameable
{
    public ExportPackage Export { get; }
    public List<ClassDef> Classes { get; }

    public PackageDef(ExportPackage export, BaseElement? parent = null) : base(parent)
    {
        Export = export;
        Classes = export.Classes.Select(cls => new ClassDef(cls, this)).ToList();
    }

    public override IEnumerable<BaseElement> Children() => Classes;
    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public string Name() => Export.Name();
}