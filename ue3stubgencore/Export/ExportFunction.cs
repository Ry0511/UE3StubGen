using UELib;
using UELib.Core;
using UELib.Flags;

namespace UE3StubGenCore.Export;

public class ExportFunction : BaseExport
{
    public bool IsStatic { get; }
    public bool IsNative { get; }
    public bool IsIterator { get; }
    public bool IsDelegate { get; }
    public bool IsOperator { get; }
    public bool HasOutParms { get; }
    public bool HasOptionalParms { get; }
    public IReadOnlyList<ExportProperty> Parameters { get; }
    public ExportProperty? ReturnParameter { get; }

    public bool IsRegularFunction => !IsIterator && !IsDelegate && !IsOperator;

    public ExportFunction(ExportContext ctx, UnrealPackage pkg, UFunction func)
        : base(pkg, func)
    {
        if (IsImport(func)) func = ctx.ResolveImport<UFunction>(func);

        IsStatic = func.FunctionFlags.HasFlag(FunctionFlag.Static);
        IsNative = func.FunctionFlags.HasFlag(FunctionFlag.Native);
        IsIterator = func.FunctionFlags.HasFlag(FunctionFlag.Iterator);
        IsDelegate = func.FunctionFlags.HasFlag(FunctionFlag.Delegate);
        IsOperator = func.FunctionFlags.HasFlag(FunctionFlag.Operator);
        HasOutParms = func.FunctionFlags.HasFlag(FunctionFlag.HasOutParms);
        HasOptionalParms = func.FunctionFlags.HasFlag(FunctionFlag.HasOptionalParms);

        Parameters = func.EnumerateFields<UProperty>()
            .Select(e => new ExportProperty(ctx, pkg, e))
            .Where(e => !e.IsReturnParam())
            .ToList();

        ReturnParameter = func.EnumerateFields<UProperty>()
            .Select(e => new ExportProperty(ctx, pkg, e))
            .FirstOrDefault(e => e.IsReturnParam());
    }
}