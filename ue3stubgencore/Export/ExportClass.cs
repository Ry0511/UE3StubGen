using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportClass : BaseExport
{
    public ExportClass? Super { get; }

    public IReadOnlyList<ExportInterface> Interfaces { get; }
    public IReadOnlyList<ExportStruct> Structs { get; }
    public IReadOnlyList<ExportEnum> Enums { get; }
    public IReadOnlyList<ExportProperty> Fields { get; }
    public IReadOnlyList<ExportFunction> Functions { get; }

    public ExportClass(ExportContext ctx, UnrealPackage pkg, UClass obj) : base(pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UClass>(obj);
        }

        if (obj.Super is UClass cls)
        {
            Super = ctx.CreateExport<ExportClass>(pkg, cls);
        }

        Interfaces = (obj.ImplementedInterfaces ?? [])
            .Select(index => ctx.ResolveImport<UClass>(obj.Package, index))
            .Select(iface => ctx.CreateExport<ExportInterface>(iface.Package, iface))
            .ToList();

        Structs = obj.EnumerateFields<UScriptStruct>()
            .Select(elem => ctx.CreateExport<ExportStruct>(pkg, elem))
            .ToList();

        Enums = obj.EnumerateFields<UEnum>()
            .Select(elem => ctx.CreateExport<ExportEnum>(pkg, elem))
            .ToList();

        Fields = obj.EnumerateFields<UProperty>()
            .Select(elem => new ExportProperty(ctx, pkg, elem))
            .ToList();

        Functions = obj.EnumerateFields<UFunction>()
            .Select(elem => new ExportFunction(ctx, pkg, elem))
            .OrderBy(GetFunctionRank)
            .ToList();
    }

    private static int GetFunctionRank(ExportFunction f)
    {
        return f switch
        {
            { IsNative: true, IsStatic: true } => 0,
            { IsStatic: true } => 1,
            { IsNative: true, IsRegularFunction: true } => 2,
            { IsRegularFunction: true } => 3,
            _ => 99
        };
    }
}