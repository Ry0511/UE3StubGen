using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class EnumDef(ExportEnum export, BaseElement? parent = null) : BaseSymbol(parent)
{
    public ExportEnum Export { get; } = export;
    public IReadOnlyList<string> Values { get; } = export.Ordinals;

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
        IsModuleUnique = Module!.NameTable[Name()].Count == 1;
    }
}