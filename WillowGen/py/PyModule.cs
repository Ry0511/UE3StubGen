using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyModule : PyBaseElement
{
    public string Name { get; } // TODO: Remove
    public List<PyClassDef> Classes { get; } = [];
    public List<PyStructDef> Structures { get; } = [];
    public List<PyEnumDef> Enums { get; } = [];

    public PyModule(ExportPackage export)
    {
        Name = export.PackageName;
        foreach (var cls in export.Classes)
        {
            Classes.Add(new PyClassDef(cls, this));

            foreach (var @struct in cls.Structs)
            {
                Structures.Add(new PyStructDef(@struct, this));
            }

            foreach (var @enum in cls.Enums)
            {
                Enums.Add(new PyEnumDef(@enum, this));
            }
        }
    }
}