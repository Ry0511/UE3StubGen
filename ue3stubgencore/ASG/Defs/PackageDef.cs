using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class PackageDef : BaseSymbol
{
    public ExportPackage Export { get; }
    public List<ClassDef> Classes { get; }
    public Dictionary<string, HashSet<BaseSymbol>> NameTable { get; }

    public PackageDef(ExportPackage export, BaseElement? parent = null)
        : base(parent)
    {
        Export = export;
        Classes = export.Classes.Select(cls => new ClassDef(cls, this)).ToList();

        // collect classes, structs, and enums defined
        NameTable = new();
        foreach (var sym in Descendants().OfType<BaseSymbol>().Where(e => e is not FunctionDef))
        {
            if (NameTable.TryGetValue(sym.Name(), out var list))
            {
                list.Add(sym);
            }
            else
            {
                NameTable[sym.Name()] = [sym];
            }
        }
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

    public override void PostEvaluate(BaseElement root)
    {
        base.PostEvaluate(root);
        IsModuleUnique = true;
        IsUniqueCrossModule = true;
    }
}
