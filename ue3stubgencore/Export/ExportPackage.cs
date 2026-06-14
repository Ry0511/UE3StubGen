using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportPackage : BaseExport
{
    public string PackageName { get; private set; }
    public List<ExportClass> Classes { get; } = new();

    public ExportPackage(ExportContext ctx, UnrealPackage pkg) : base(ctx, pkg, pkg.RootPackage)
    {
        PackageName = pkg.PackageName;

        foreach (var cls in pkg.Objects.OfType<UClass>())
        {
            if (!IsExport(cls)) continue;

            try
            {
                Classes.Add(ctx.CreateExport<ExportClass>(pkg, cls));
            }
            catch (Exception err)
            {
                Console.WriteLine($"Error creating export class for {cls.Name}: {err.Message}");
            }
        }
    }
}