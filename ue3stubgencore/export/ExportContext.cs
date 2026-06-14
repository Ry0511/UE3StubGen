using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

/**
 * Export context that holds all loaded packages during the export process, this includes packages
 * loaded from import resolution.
 */
public class ExportContext
{
    private DirectoryInfo _root { get; }
    private readonly Dictionary<string, UnrealPackage> _packages = new();

    public ExportContext(string path, bool loadAll = true)
    {
        _root = new DirectoryInfo(path);
        if (!_root.Exists)
        {
            throw new Exception($"path specified does not exist: {path}");
        }

        if (loadAll)
        {
            foreach (var file in _root.GetFiles("*.u", SearchOption.AllDirectories))
            {
                try
                {
                    var pkg = UnrealLoader.LoadPackage(file.FullName);
                    pkg.InitializePackage();
                    AddPackage(pkg);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load package {file.Name}: {e}");
                }
            }
        }
    }

    private FileInfo? FindPackageByName(string name)
    {
        return _root.GetFiles("*.u", SearchOption.AllDirectories)
            .FirstOrDefault(file =>
                file.Name.Equals(name + ".u", StringComparison.CurrentCultureIgnoreCase));
    }

    private void AddPackage(UnrealPackage pkg)
    {
        _packages[pkg.PackageName] = pkg;
    }

    public UnrealPackage? LoadPackage(string name)
    {
        return GetPackage(name, loadIfMissing: true);
    }

    public UnrealPackage? GetPackage(string name, bool loadIfMissing = true)
    {
        var ret = _packages.GetValueOrDefault(name);
        if (ret == null && loadIfMissing)
        {
            var file = FindPackageByName(name);
            if (file != null)
            {
                var pkg = UnrealLoader.LoadPackage(file.FullName);
                pkg.InitializePackage();
                AddPackage(pkg);
                return pkg;
            }
        }

        return ret;
    }

    public T? ResolveImport<T>(UnrealPackage pkg, UObject obj) where T : UObject
    {
        var index = (int)obj;
        switch (index)
        {
            case 0:
                throw new Exception("null import cannot be resolved");
            case > 0:
                return obj as T;
            default:
            {
                var imp = pkg.Imports[-(index + 1)];
                return GetPackage(imp.ClassPackageName, loadIfMissing: true)
                    ?.FindObject<T>(imp.ObjectName);
            }
        }
    }
}