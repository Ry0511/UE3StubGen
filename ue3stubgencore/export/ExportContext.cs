using System.Globalization;
using UELib;
using UELib.Core;
using UELib.Flags;
using static UELib.UnrealPackage.GameBuild.BuildName;

namespace ue3stubgencore.export;

/**
 * Export context that holds all loaded packages during the export process, this includes packages
 * loaded from import resolution.
 */
public class ExportContext
{
    private DirectoryInfo _root { get; }
    private readonly Dictionary<string, UnrealPackage> _packages = new();

    public ExportContext(string path)
    {
        _root = new DirectoryInfo(path);
        if (!_root.Exists)
        {
            throw new Exception($"path specified does not exist: {path}");
        }

        var packageList = _root.GetFiles("*.u", SearchOption.AllDirectories)
            .Select(file => file)
            .ToList();

        var seenSet = new HashSet<string>();

        foreach (var pkgPath in packageList)
        {
            UnrealConfig.SuppressSignature = true;
            var pkg = UnrealLoader.LoadPackage(pkgPath.FullName);
            UnrealConfig.SuppressSignature = false;

            // this is a compressed package
            if (pkg.Summary.CompressedChunks != null && pkg.Summary.CompressedChunks.Any())
            {
                Console.WriteLine($"Skipping {pkgPath.Name} because it is compressed");
                pkg.Stream.Close();
                continue;
            }

            if (!seenSet.Add(pkg.PackageName))
            {
                pkg.Stream.Close();
                continue;
            }
            
            string ntlPath = Path.Combine(
                AppContext.BaseDirectory,
                "NativeTables",
                "NativesTableList_UDK-2012-05.NTL"
            );
            
            if (File.Exists(ntlPath))
            {
                using var ntlFileStream = File.Open(ntlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                pkg.NTLPackage = new NativesTablePackage();
                pkg.NTLPackage.Deserialize(ntlFileStream);
            }
            
            pkg.InitializePackage();
            AddPackage(pkg);
        }
    }

    private void AddPackage(UnrealPackage pkg)
    {
        _packages[pkg.PackageName] = pkg;
    }

    public UnrealPackage? GetPackage(string name)
    {
        return _packages.GetValueOrDefault(name);
    }

    public T ResolveImport<T>(UnrealPackage pkg, UObject obj) where T : UObject
    {
        var index = (int)obj;
        switch (index)
        {
            case 0:
                throw new Exception("null import cannot be resolved");
            case > 0:
                return (obj as T)!;
            default:
            {
                var imp = pkg.Imports[-(index + 1)];
                var loaded = GetPackage(imp.ClassPackageName);
                var found = loaded?.FindObject<T>(imp.ObjectName);

                if (found == null)
                {
                    throw new Exception(
                        $"Could not resolve import: {imp.GetReferencePath()} from {imp.ClassPackageName}");
                }

                return found;
            }
        }
    }
}