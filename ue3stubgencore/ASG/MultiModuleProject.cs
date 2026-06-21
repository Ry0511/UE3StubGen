using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;

namespace UE3StubGenCore.ASG;

public class MultiModuleProject : BaseElement
{
    public IReadOnlyList<PackageDef> Modules { get; }

    public SymbolTable Symbols { get; } = new ();

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
            var visible = cls.Functions.Concat(cls.InheritedTypes().SelectMany(elem => elem.Functions));

            foreach (var group in visible.GroupBy(func => func.SignatureKey()))
            {
                FunctionDef? anchor = null;
                foreach (var func in group)
                {
                    if (anchor == null)
                    {
                        anchor = func;
                    }
                    else
                    {
                        Union(anchor, func);
                    }
                }
            }
        }

        foreach (var family in parent.Keys.GroupBy(Find))
        {
            var members = family.ToList();
            var distinct = members
                .Select(func => string.Join("\0", func.Params.Select(p => p.Name())))
                .Distinct()
                .Count();

            if (distinct > 1)
            {
                foreach (var func in members)
                {
                    func.FamilyHasNaughtyOverride = true;
                }
            }
        }

        return;

        FunctionDef Find(FunctionDef func)
        {
            if (!parent.TryGetValue(func, out var p))
            {
                parent[func] = func;
                return func;
            }

            return p == func ? func : parent[func] = Find(p);
        }

        void Union(FunctionDef lhs, FunctionDef rhs)
        {
            var ra = Find(lhs);
            var rb = Find(rhs);
            if (ra != rb)
            {
                parent[ra] = rb;
            }
        }
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

    public override IEnumerable<BaseElement> Children()
    {
        return Modules;
    }
}
