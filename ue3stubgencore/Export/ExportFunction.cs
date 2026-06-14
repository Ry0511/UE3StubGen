using UELib;
using UELib.Core;
using UELib.Flags;

namespace UE3StubGenCore.Export;

public class ExportFunction : BaseExport
{
    public string Name { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsNative { get; private set; }
    public bool IsIterator { get; }
    public bool IsDelegate { get; }
    public bool IsOperator { get; }
    public bool IsRegularFunction => !IsIterator && !IsDelegate && !IsOperator;
    public bool HasOutParms { get; private set; }
    public bool HasOptionalParms { get; private set; }
    public List<ExportField> Parameters { get; } = [];
    public ExportField? ReturnParameter { get; }
    public bool HasReturnParameter => ReturnParameter != null;
    public IEnumerable<ExportField> OutParameters => Parameters.Where(x => x.IsOutParm);

    public ExportFunction(ExportContext ctx, UnrealPackage pkg, UFunction func)
        : base(ctx, pkg, func)
    {
        if (IsImport(func))
        {
            func = ctx.ResolveImport<UFunction>(func);
        }

        Name = func.FriendlyName;
        IsStatic = func.FunctionFlags.HasFlag(FunctionFlag.Static);
        IsNative = func.FunctionFlags.HasFlag(FunctionFlag.Native);
        IsIterator = func.FunctionFlags.HasFlag(FunctionFlag.Iterator);
        IsDelegate = func.FunctionFlags.HasFlag(FunctionFlag.Delegate);
        IsOperator = func.FunctionFlags.HasFlag(FunctionFlag.Operator);
        HasOutParms = func.FunctionFlags.HasFlag(FunctionFlag.HasOutParms);
        HasOptionalParms = func.FunctionFlags.HasFlag(FunctionFlag.HasOptionalParms);

        foreach (var elem in func.EnumerateFields<UProperty>())
        {
            ExportField f = new(ctx, pkg, elem);
            if (!f.IsReturnParm)
            {
                Parameters.Add(f);
            }
            else
            {
                if (ReturnParameter != null)
                {
                    throw new Exception("Multiple return parameters?");
                }
                ReturnParameter = f;
            }
        }
    }
}