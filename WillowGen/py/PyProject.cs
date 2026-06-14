using UE3StubGenCore.Render;
using UELib.Core;

namespace WillowGen.py;

public class PyProject : PyBaseElement
{
    public IReadOnlyList<PyModule> Modules { get; }
    public SymbolTable Symbols { get; } = new();

    public PyProject(ExportModel model)
    {
        Modules = model.Packages.Select(elem => new PyModule(elem, this)).ToList();
        LoadSymbols();
    }

    public void LoadSymbols()
    {
        foreach (var sym in Descendants().OfType<ISymbol>()) Symbols.Register(sym);
        foreach (var @ref in Descendants().OfType<PyRef>())
        {
            if (!@ref.ReferencesBuiltin && Symbols.Resolve(@ref) == null)
            {
                if (@ref.Export.ObjectHandle is UObjectProperty)
                {
                    // TODO: some of these just don't exist in the decompiled output so just emit
                    //  `type Bla = UObject | None` or similar
                    continue;
                }

                Console.WriteLine($"could not resolve {@ref.FullPath} ({@ref.DirectInit}, {@ref.ReferencesBuiltin}, {@ref.Export.ObjectHandle.GetReferencePath()})");
            }
        }
    }

    public override IEnumerable<PyBaseElement> Children() => Modules;
}