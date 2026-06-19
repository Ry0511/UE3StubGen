using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportContext
{
    private readonly DirectoryInfo _packageRoot;
    private readonly Dictionary<string, UnrealPackage> _packages = new();
    private readonly Dictionary<string, BaseExport> _exportCache = new();

    public int CacheCount => _exportCache.Count;

    private void LoadAndInitialiseAllPackages()
    {
        var packageList = _packageRoot.GetFiles("*.u", SearchOption.AllDirectories)
            .Select(file => file)
            .ToList();

        var consoleOut = Console.Out;
        var seenSet = new HashSet<string>();

        var ntlPath = Path.Combine(
            AppContext.BaseDirectory,
            "NativeTables",
            "NativesTableList_UDK-2012-05.NTL"
        );

        NativesTablePackage? ntlPackage = null;

        if (File.Exists(ntlPath))
        {
            using var ntlFileStream =
                File.Open(ntlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            ntlPackage = new NativesTablePackage();
            ntlPackage.Deserialize(ntlFileStream);
        }
        else
        {
            Console.WriteLine("failed to find NTL package file - names might be missing");
        }

        foreach (var pkgPath in packageList)
            try
            {
                Console.SetOut(TextWriter.Null);
                UnrealConfig.SuppressSignature = true;
                var pkg = UnrealLoader.LoadPackage(pkgPath.FullName);
                UnrealConfig.SuppressSignature = false;
                if (ntlPackage != null) pkg.NTLPackage = ntlPackage;

                pkg.InitializePackage();
                Console.SetOut(consoleOut);
                Console.WriteLine($"Loaded Package {pkgPath.FullName}");

                if (pkg.Summary.CompressedChunks != null && pkg.Summary.CompressedChunks.Any())
                {
                    Console.WriteLine(
                        $"Skipping {pkgPath.Name} because it is a compressed package");
                    pkg.Stream.Close();
                    continue;
                }

                if (!seenSet.Add(pkg.PackageName))
                {
                    pkg.Stream.Close();
                    continue;
                }

                AddPackage(pkg);
            }
            catch (Exception err)
            {
                Console.SetOut(consoleOut);
                Console.WriteLine($"Failed to load {pkgPath.Name}: {err.Message}");
                throw;
            }
            finally
            {
                Console.SetOut(consoleOut);
            }
    }

    public ExportContext(string path)
    {
        _packageRoot = new DirectoryInfo(path);
        if (!_packageRoot.Exists) throw new Exception($"path specified does not exist: {path}");

        _exportCache.EnsureCapacity(8192);
        LoadAndInitialiseAllPackages();
    }

    private void AddPackage(UnrealPackage pkg)
    {
        _packages[pkg.PackageName] = pkg;
    }

    public UnrealPackage? GetPackage(string name)
    {
        return _packages.GetValueOrDefault(name);
    }

    public IEnumerable<UnrealPackage> GetPackages()
    {
        return _packages.Values;
    }

    public T ResolveImport<T>(UObject obj, bool checkSubclasses = false) where T : UObject
    {
        var index = (int)obj;
        if (index > 0) return (obj as T)!;

        return ResolveImport<T>(obj.Package, index, checkSubclasses);
    }

    public T ResolveImport<T>(UnrealPackage pkg, int index, bool checkSubclasses = false) where T : UObject
    {
        // this is an export, so just load it from package
        if (index > 0)
        {
            var export = pkg.Exports[index - 1];
            if (export.Object is T obj) return obj;

            throw new Exception("export is null or of different type");
        }

        // needs to be imported from another package
        if (index < 0)
        {
            var imp = pkg.Imports[-(index + 1)];

            var packageToImport = imp.Outer;
            while (packageToImport?.Outer != null) packageToImport = packageToImport.Outer;

            var loaded = GetPackage(packageToImport!.ObjectName);
            var found = loaded?.FindObject<T>(imp.ObjectName, checkSubclasses);

            if (found == null)
                throw new Exception(
                    $"Could not resolve import: {imp.GetReferencePath()} from {packageToImport!.ObjectName}");

            return found;
        }

        throw new Exception("null import cannot be resolved");
    }

    public T CreateExport<T>(UnrealPackage pkg, UObject obj) where T : BaseExport
    {
        if (_exportCache.TryGetValue(obj.GetPath(), out var cachedExport))
        {
            if (cachedExport is T cachedExportAs) return cachedExportAs;

            throw new Exception("cached export is of a different type or is null");
        }

        static bool IsInterface(UClass cls)
        {
            var super = cls.Super;
            while (super != null && super.Super != null) super = super.Super;

            return super != null && string.Compare(super.Name, "Interface", StringComparison.OrdinalIgnoreCase) == 0;
        }

        BaseExport createdExport = obj switch
        {
            UClass elem => IsInterface(elem)
                ? new ExportInterface(this, pkg, elem)
                : new ExportClass(this, pkg, elem),
            UEnum elem => new ExportEnum(this, pkg, elem),
            UFunction elem => new ExportFunction(this, pkg, elem),
            UProperty elem => new ExportProperty(this, pkg, elem),
            UStruct elem => new ExportStruct(this, pkg, elem),
            _ => throw new Exception($"unsupported object type: {obj.GetType().Name}")
        };

        if (createdExport is not T createdExportAs) throw new Exception("export is not of expected type");

        _exportCache.Add(obj.GetPath(), createdExportAs);
        return createdExportAs;
    }
}