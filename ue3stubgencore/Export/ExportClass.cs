using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportClass : BaseExport
{
    public string Name { get; private set; }
    public ExportClass? Super { get; private set; }
    public List<ExportInterface> Interfaces { get; } = [];
    public List<ExportStruct> Structs { get; private set; }
    public List<ExportEnum> Enums { get; private set; }
    public List<ExportField> Fields { get; private set; }
    public List<ExportFunction> Functions { get; }

    public ExportClass(ExportContext ctx, UnrealPackage pkg, UClass obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UClass>(obj);
        }

        Name = obj.Name; // TODO: determine the difference between Name and FriendlyName

        if (obj.Super != null)
        {
            if (obj.Super is UClass cls)
            {
                Super = ctx.CreateExport<ExportClass>(pkg, cls);
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
                var iface = ctx.ResolveImport<UClass>(obj.Package, index);
                Interfaces.Add(ctx.CreateExport<ExportInterface>(iface.Package, iface));
            }
            catch (Exception err)
            {
                Console.WriteLine($"Failed to resolve interface {index}: {err.Message} - skipping this one");
            }
        }

        Structs = obj.EnumerateFields<UScriptStruct>()
            .Select(elem => ctx.CreateExport<ExportStruct>(pkg, elem))
            .ToList();

        Enums = obj.EnumerateFields<UEnum>()
            .Select(elem => ctx.CreateExport<ExportEnum>(pkg, elem))
            .ToList();

        Fields = obj.EnumerateFields<UProperty>()
            .Select(elem => new ExportField(ctx, pkg, elem))
            .ToList();

        Functions = obj.EnumerateFields<UFunction>()
            .Select(elem => new ExportFunction(ctx, pkg, elem))
            .ToList();
        
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