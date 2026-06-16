using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;

namespace UE3StubGenCore.ASG;

public class MultiModuleProject : BaseElement
{
    public IReadOnlyList<PackageDef> Modules { get; }
    public SymbolTable Symbols { get; } = new();

    public MultiModuleProject(ExportModel model)
    {
        Modules = model.Packages.Select(elem => new PackageDef(elem, this)).ToList();
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
        foreach (var symbolRef in Descendants().OfType<RefNode>())
        {
            symbolRef.ResolvedTo = Symbols.Resolve(symbolRef);
        }
    }

    public override IEnumerable<BaseElement> Children() => Modules;
}