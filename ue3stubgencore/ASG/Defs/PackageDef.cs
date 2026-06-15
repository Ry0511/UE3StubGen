using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class PackageDef : BaseElement, ISymbol
{
    
    public ExportPackage Export { get; }
    public List<ClassDef> Classes { get; } = [];
    public List<StructDef> Structures { get; } = [];
    public List<EnumDef> Enums { get; } = [];

    public PackageDef(ExportPackage export, BaseElement? parent = null) : base(parent)
    {
        Export = export;
        foreach (var cls in export.Classes)
        {
            Classes.Add(new ClassDef(cls, this));

            foreach (var @struct in cls.Structs)
            {
                Structures.Add(new StructDef(@struct, this));
            }

            foreach (var @enum in cls.Enums)
            {
                Enums.Add(new EnumDef(@enum, this));
            }
        }
    }

    public override IEnumerable<BaseElement> Children()
    {
        foreach (var elem in Classes) yield return elem;
        foreach (var elem in Structures) yield return elem;
        foreach (var elem in Enums) yield return elem;
    }

    public string ExportPathName() => Export.ObjectHandle.GetPath();
    public bool CanBeReferenced() => false;
}