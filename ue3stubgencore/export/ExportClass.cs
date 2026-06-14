using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

public class ExportClass : BaseExport
{
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

        foreach (var index in obj.ImplementedInterfaces ?? [])
        {
            // when index=0 it is a null import/export
            var i = index > 0 ? index - 1 : -(index + 1);

            switch (index)
            {
                case > 0 when pkg.Exports[i].Object is UClass ifaceCls:
                    Interfaces.Add(new ExportInterface(ctx, pkg, ifaceCls));
                    break;
                case > 0:
                    throw new Exception("Interface is not a class?");
                case < 0:
                {
                    var import = pkg.Imports[i];
                    Console.WriteLine($"Import From: {import.ObjectPackageName}");
                    break;
                }
                default:
                    throw new Exception("interface export is null");
            }
        }

        Structs = obj.EnumerateFields<UStruct>().Select(s => new ExportStruct(ctx, pkg, s))
            .ToList();
        Enums = obj.EnumerateFields<UEnum>().Select(e => new ExportEnum(ctx, pkg, e)).ToList();
        Fields = obj.EnumerateFields<UProperty>().Select(f => new ExportField(ctx, pkg, f))
            .ToList();
        Functions = obj.EnumerateFields<UFunction>().Select(f => new ExportFunction(ctx, pkg, f))
            .ToList();
    }
}