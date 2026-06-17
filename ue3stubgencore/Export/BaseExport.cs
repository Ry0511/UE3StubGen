using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public abstract class BaseExport(UnrealPackage pkg, UObject obj)
{
    public static bool IsImport(int index) => index < 0;
    public static bool IsImport(UObject obj) => IsImport((int)obj);
    public static bool IsExport(UObject obj) => (int)obj > 0;

    public UnrealPackage Package { get; } = pkg;
    public UObject ObjectHandle { get; } = obj;

    public string GetPath() => ObjectHandle.GetPath();
    public string PackageName() => Package.PackageName;
    public string Name() => ObjectHandle.Name;
}