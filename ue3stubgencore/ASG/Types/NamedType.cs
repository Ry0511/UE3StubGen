using UE3StubGenCore.ASG.Defs;

namespace UE3StubGenCore.ASG.Types;

public class NamedType : BaseType
{
    public RefNode Ref { get; }

    public NamedType(string fullPath, BaseElement? parent)
        : base(parent)
    {
        Ref = new RefNode(fullPath, this);
    }

    public override IEnumerable<BaseElement> Children()
    {
        yield return Ref;
    }

    public override string Name()
    {
        return Ref.TargetFullPath;
    }

    public bool IsClassRef()
    {
        if (Ref.ResolvedTo == null)
        {
            return false;
        }

        var r = Ref.ResolvedTo;
        return r is ClassDef;
    }

    public bool IsStructRef()
    {
        if (Ref.ResolvedTo == null)
        {
            return false;
        }

        var r = Ref.ResolvedTo;
        return r is StructDef;
    }

    public bool IsEnumRef()
    {
        if (Ref.ResolvedTo == null)
        {
            return false;
        }

        var r = Ref.ResolvedTo;
        return r is EnumDef;
    }
}
