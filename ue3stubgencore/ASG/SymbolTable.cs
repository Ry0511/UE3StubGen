namespace UE3StubGenCore.ASG;

public class SymbolTable
{
    private readonly Dictionary<string, ISymbol> _fullPathSymbols = new();

    public void Register(ISymbol sym)
    {
        if (sym.CanBeReferenced())
        {
            _fullPathSymbols.Add(sym.ExportPathName(), sym);
        }
    }

    public ISymbol? Resolve(string fullPath) => _fullPathSymbols.GetValueOrDefault(fullPath);
    public ISymbol? Resolve(RefNode refNode) => Resolve(refNode.TargetFullPath);
}