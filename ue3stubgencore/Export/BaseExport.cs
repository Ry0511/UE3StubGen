using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public abstract class BaseExport(UnrealPackage pkg, UObject obj)
{
    public static bool IsImport(int index)
    {
        return index < 0;
    }

    public static bool IsImport(UObject obj)
    {
        return IsImport((int)obj);
    }

    public static bool IsExport(UObject obj)
    {
        return (int)obj > 0;
    }

    public UnrealPackage Package { get; } = pkg;
    public UObject ObjectHandle { get; } = obj;

    public string GetPath()
    {
        return ObjectHandle.GetPath();
    }

    public string PackageName()
    {
        return Package.PackageName;
    }

    public string Name()
    {
        return ObjectHandle.Name;
    }
}