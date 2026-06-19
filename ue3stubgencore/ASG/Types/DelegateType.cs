using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class DelegateType : BaseType
{
    public RefNode Function { get; }

    public DelegateType(UDelegateProperty prop, BaseElement? parent = null) : base(parent)
    {
        // no idea what the 'Delegate' parameter is for, seems its only non-null when used as
        //  a function parameter; keeping this around for now but BL1 seems safe
        if (prop.Delegate != null && prop.Delegate != prop.Function)
        {
            throw new Exception("Delegate != Function");
        }

        Function = new RefNode(prop.Function.GetPath(), this);
    }

    public override string Name() => $"delegate<{Function.ResolvedTo?.Name() ?? "NULL"}>";

    public override IEnumerable<BaseElement> Children()
    {
        yield return Function;
    }
}