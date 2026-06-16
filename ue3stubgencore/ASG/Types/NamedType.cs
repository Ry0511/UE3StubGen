namespace UE3StubGenCore.ASG.Types;

public class NamedType : BaseType
{
    public RefNode Ref { get; }

    public NamedType(string fullPath, BaseElement? parent) : base(parent)
    {
        Ref = new RefNode(fullPath, this);
    }

    public override IEnumerable<BaseElement> Children()
    {
        yield return Ref;
    }
}