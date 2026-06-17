using UELib.Core;

namespace UE3StubGenCore.ASG.Types;

public class StaticArrayType : BaseType
{
    public BaseType HeldType { get; }
    public int ArrayDim { get; }

    public StaticArrayType(UProperty prop, BaseElement? parent = null) : base(parent)
    {
        if (prop.ArrayDim <= 1)
        {
            throw new Exception("ArrayDim <= 1");
        }

        HeldType = Create(prop, this, ignoreArrayDim: true);
        ArrayDim = prop.ArrayDim;
    }

    public override IEnumerable<BaseElement> Children()
    {
        yield return HeldType;
    }

    public override string Name() => $"{HeldType.Name()}[{ArrayDim}]";
}