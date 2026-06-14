using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

public abstract class BaseExport(ExportContext ctx, UnrealPackage pkg, UObject obj)
{
    public ExportContext Context { get; private set; } = ctx;
    public UnrealPackage Package { get; private set; } = pkg;
    public UObject ObjectHandle { get; private set; } = obj;

    public static bool IsImport(int index) => index < 0;
    public static bool IsImport(UObject obj) => IsImport((int)obj);
}