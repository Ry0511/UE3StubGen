using UE3StubGenCore.Render;

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
        // walk through all modules and register every symbol that can be referenced
        foreach (var sym in Descendants().OfType<ISymbol>())
        {
            Symbols.Register(sym);
        }

        // walk through all references and resolve them
        foreach (var symbolRef in Descendants().OfType<PyRef>())
        {
            // This resolution can fail i.e., IntProperty is likely not a symbol, it is a builtin
            symbolRef.ResolvedTo = Symbols.Resolve(symbolRef) as PyBaseElement;
        }
    }

    public override IEnumerable<PyBaseElement> Children() => Modules;
}