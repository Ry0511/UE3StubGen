using System.Text;
using UE3StubGenCore.ASG.Defs;
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

    public override IEnumerable<BaseElement> Children()
    {
        yield return Function;
    }

    public override string Name()
    {
        if (Function.ResolvedTo is not FunctionDef func)
        {
            return $"Delegate<{Function.GetHashCode()}>";
        }

        // TODO: delegates can be treated as other delegates of the same shape i.e.,
        //    delegate A(int B) == delegate B(int C)
        //  but the Name() from the export will return A or B so it can't be used here as we use
        //  this to determine 'is-same'...
        StringBuilder sb = new();
        sb.Append("Delegate,");
        sb.Append($"{func.Params.Count},");
        sb.Append('(');

        foreach (var param in func.Params)
        {
            sb.Append(param.Name() + ";" + param.ParamType.Name());
            sb.Append(',');
        }

        sb.Append(')');
        return sb.ToString();
    }
}