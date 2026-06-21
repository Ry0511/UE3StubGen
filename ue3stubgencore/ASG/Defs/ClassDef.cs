using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class ClassDef : BaseSymbol
{
    public ExportClass Export { get; }

    public RefNode? Super { get; }

    public IReadOnlyList<RefNode> Interfaces { get; }

    public IReadOnlyList<EnumDef> Enums { get; }

    public IReadOnlyList<StructDef> Structs { get; }

    public IReadOnlyList<TypedParamDef> Fields { get; }

    public IReadOnlyList<FunctionDef> Functions { get; }

    public ClassDef(ExportClass export, BaseElement? parent)
        : base(parent)
    {
        Export = export;
        if (export.Super != null)
        {
            Super = new RefNode(export.Super.GetPath(), this);
        }

        Interfaces = export.Interfaces.Select(elem => new RefNode(elem.GetPath(), this)).ToList();
        Enums = export.Enums.Select(elem => new EnumDef(elem, this)).ToList();
        Structs = export.Structs.Select(elem => new StructDef(elem, this)).ToList();
        Fields = export.Fields.Select(elem => new TypedParamDef(elem, this)).ToList();

        Functions = export
            .Functions.Where(elem => elem.IsRegularFunction || elem.IsDelegate)
            .Select(elem => new FunctionDef(elem, this))
            .ToList();
    }

    public override IEnumerable<BaseElement> Children()
    {
        if (Super != null)
        {
            yield return Super;
        }

        foreach (var elem in Interfaces)
        {
            yield return elem;
        }

        foreach (var elem in Enums)
        {
            yield return elem;
        }

        foreach (var elem in Structs)
        {
            yield return elem;
        }

        foreach (var elem in Fields)
        {
            yield return elem;
        }

        foreach (var elem in Functions)
        {
            yield return elem;
        }
    }

    public override string ExportPathName()
    {
        return Export.ObjectHandle.GetPath();
    }

    public override string Name()
    {
        return Export.Name();
    }

    public IEnumerable<ClassDef> InheritedTypes()
    {
        var seen = new HashSet<ClassDef>();
        var stack = new Stack<ClassDef>();

        Push(Super?.ResolvedTo as ClassDef);
        foreach (var iface in Interfaces)
        {
            Push(iface.ResolvedTo as ClassDef);
        }

        while (stack.Count > 0)
        {
            var c = stack.Pop();
            yield return c;

            Push(c.Super?.ResolvedTo as ClassDef);
            foreach (var iface in c.Interfaces)
            {
                Push(iface.ResolvedTo as ClassDef);
            }
        }

        yield break;

        void Push(ClassDef? c)
        {
            if (c != null && seen.Add(c))
            {
                stack.Push(c);
            }
        }
    }

    public override void PostEvaluate(BaseElement root)
    {
        base.PostEvaluate(root);
        IsModuleUnique = Module!.NameTable[Name()].Count == 1;
    }
}
