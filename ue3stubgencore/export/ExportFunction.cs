using UELib;
using UELib.Core;
using UELib.Flags;

namespace ue3stubgencore.export;

public class ExportFunction : BaseExport
{
    public string Name { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsNative { get; private set; }
    public bool IsIterator { get; private set; }
    public bool IsDelegate { get; private set; }
    public bool HasOutParms { get; private set; }
    public bool HasOptionalParms { get; private set; }
    public List<ExportField> Parameters { get; } = [];
    public ExportField? ReturnParameter { get; }
    public bool HasReturnParameter => ReturnParameter != null;

    public ExportFunction(ExportContext ctx, UnrealPackage pkg, UFunction func) : base(ctx, pkg,
        func)
    {
        if (IsImport(func))
        {
            func = ctx.ResolveImport<UFunction>(pkg, func)!;
        }

        Name = func.FriendlyName;
        IsStatic = func.FunctionFlags.HasFlag(FunctionFlag.Static);
        IsNative = func.FunctionFlags.HasFlag(FunctionFlag.Native);
        IsIterator = func.FunctionFlags.HasFlag(FunctionFlag.Iterator);
        IsDelegate = func.FunctionFlags.HasFlag(FunctionFlag.Delegate);
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
                    throw new Exception("Multiple return parameters?");
                ReturnParameter = f;
            }
        }
    }
}