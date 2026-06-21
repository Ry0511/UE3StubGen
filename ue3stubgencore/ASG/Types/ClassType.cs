using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class ClassType : BaseType
{
    public NamedType? MetaClass { get; }

    public ClassType(UClassProperty prop, BaseElement? parent)
        : base(parent)
    {
        if (prop.MetaClass != null)
        {
            MetaClass = new NamedType(prop.MetaClass.GetPath(), this);
        }
    }

    public override IEnumerable<BaseElement> Children()
    {
        if (MetaClass != null)
        {
            yield return MetaClass;
        }
    }

    public override string Name()
    {
        return MetaClass != null ? $"Class<{MetaClass.Name()}>" : "Class";
    }
}
