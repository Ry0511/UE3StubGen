using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

public class ExportClass : BaseExport
{
    public string Name { get; private set; }
    public ExportClass? Super { get; private set; } = null;
    public List<ExportInterface> Interfaces { get; private set; } = [];
    public List<ExportStruct> Structs { get; private set; }
    public List<ExportEnum> Enums { get; private set; }
    public List<ExportField> Fields { get; private set; }
    public List<ExportFunction> Functions { get; private set; }

    public ExportClass(ExportContext ctx, UnrealPackage pkg, UClass obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UClass>(pkg, obj)!;
        }

        Name = obj.Name; // TODO: determine the difference between Name and FriendlyName

        if (obj.Super != null)
        {
            if (obj.Super is UClass cls)
            {
                Super = new ExportClass(ctx, pkg, cls);
            }
            else
            {
                throw new Exception("Super is not a class?");
            }
        }

        // interfaces are indices into the import/export table so need a lookup
        foreach (var index in obj.ImplementedInterfaces ?? [])
        {
            try
            {
                var iface = ctx.ResolveImport<UClass>(pkg, index);
                Interfaces.Add(new ExportInterface(ctx, iface.Package, iface));
            }
            catch (Exception err)
            {
                Console.WriteLine($"Failed to resolve interface {index}: {err.Message} - skipping this one");
            }
        }

        Structs = obj.EnumerateFields<UStruct>().Select(s => new ExportStruct(ctx, pkg, s)).ToList();
        Enums = obj.EnumerateFields<UEnum>().Select(e => new ExportEnum(ctx, pkg, e)).ToList();
        Fields = obj.EnumerateFields<UProperty>().Select(f => new ExportField(ctx, pkg, f)).ToList();

        Functions = obj.EnumerateFields<UFunction>().Select(f => new ExportFunction(ctx, pkg, f)).ToList();
        Functions.Sort((a, b) =>
        {
            return GetFunctionRank(a).CompareTo(GetFunctionRank(b));
            static int GetFunctionRank(ExportFunction f)
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
        });
    }

    public bool IsBuiltinClass => ObjectHandle.Outer != null && ObjectHandle.Outer!.Name == "Core";
    public bool HasInterfaces() => Interfaces.Count > 0;
}