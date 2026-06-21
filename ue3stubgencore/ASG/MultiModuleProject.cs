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

        var seen = new HashSet<BaseElement>();
        foreach (var elem in Descendants())
        {
            if (seen.Add(elem))
            {
                elem.PostEvaluate(this);
            }
        }

        ResolvePositionalParameters();
    }

    private void ResolvePositionalParameters()
    {
        var parent = new Dictionary<FunctionDef, FunctionDef>();

        foreach (var cls in Descendants().OfType<ClassDef>())
        {
            var visible = cls.Functions.Concat(cls.InheritedTypes().SelectMany(t => t.Functions));

            foreach (var group in visible.GroupBy(f => f.SignatureKey()))
            {
                FunctionDef? anchor = null;
                foreach (var f in group)
                {
                    if (anchor == null)
                        anchor = f;
                    else
                        Union(anchor, f);
                }
            }
        }

        foreach (var family in parent.Keys.GroupBy(Find))
        {
            var members = family.ToList();
            var distinct = members
                .Select(f => string.Join("\0", f.Params.Select(p => p.Name())))
                .Distinct()
                .Count();

            if (distinct > 1)
                foreach (var f in members)
                    f.FamilyHasNaughtyOverride = true;
        }

        return;

        FunctionDef Find(FunctionDef f)
        {
            if (!parent.TryGetValue(f, out var p))
            {
                parent[f] = f;
                return f;
            }

            return p == f ? f : parent[f] = Find(p);
        }

        void Union(FunctionDef a, FunctionDef b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (ra != rb)
                parent[ra] = rb;
        }
    }

    public void LoadSymbols()
    {
        // walk through all modules and register every symbol that can be referenced
        foreach (var sym in Descendants().OfType<BaseSymbol>())
            Symbols.Register(sym);

        // walk through all references and resolve them
        foreach (var symbolRef in Descendants().OfType<RefNode>())
        {
            var x = Symbols.Resolve(symbolRef);
            symbolRef.ResolvedTo = x;
        }
    }

    public override IEnumerable<BaseElement> Children()
    {
        return Modules;
    }
}
