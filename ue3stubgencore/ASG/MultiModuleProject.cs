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
        LinkOverrides();
    }

    public void LoadSymbols()
    {
        // walk through all modules and register every symbol that can be referenced
        foreach (var sym in Descendants().OfType<BaseSymbol>())
        {
            Symbols.Register(sym);
        }

        // walk through all references and resolve them
        foreach (var symbolRef in Descendants().OfType<RefNode>())
        {
            var x = Symbols.Resolve(symbolRef);
            symbolRef.ResolvedTo = x;
        }
    }

    public void LinkOverrides()
    {
        foreach (var cls in Descendants().OfType<ClassDef>())
        foreach (var fn in cls.Functions.Where(f => !f.IsDelegate))
        {
            fn.IsOverride = cls.InheritedTypes().Any(inherited =>
                inherited.Functions.Any(b =>
                    !b.IsDelegate
                    && string.Equals(b.Name(), fn.Name(), StringComparison.OrdinalIgnoreCase)
                    && b.HasSameSignatureAs(fn)));
        }
    }

    public override IEnumerable<BaseElement> Children()
    {
        return Modules;
    }
}