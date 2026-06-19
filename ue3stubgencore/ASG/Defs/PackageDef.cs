using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

// TODO: should this be a symbol?
public class PackageDef : BaseSymbol
{
    public ExportPackage Export { get; }
    public List<ClassDef> Classes { get; }

    public PackageDef(ExportPackage export, BaseElement? parent = null) : base(parent)
    {
        Export = export;
        Classes = export.Classes.Select(cls => new ClassDef(cls, this)).ToList();
    }

    public override IEnumerable<BaseElement> Children()
    {
        return Classes;
    }

    public override string ExportPathName()
    {
        return Export.ObjectHandle.GetPath();
    }

    public override string Name()
    {
        return Export.Name();
    }
}