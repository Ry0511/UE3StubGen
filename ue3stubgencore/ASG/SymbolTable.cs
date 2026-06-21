namespace UE3StubGenCore.ASG;

public class SymbolTable
{
    private readonly Dictionary<string, BaseSymbol> fullPathSymbols = new ();

    public void Register(ISymbol sym)
    {
        if (sym is BaseSymbol elem)
        {
            fullPathSymbols.Add(sym.ExportPathName(), elem);
        }
    }

    public BaseSymbol? Resolve(string fullPath)
    {
        return fullPathSymbols.GetValueOrDefault(fullPath);
    }

    public BaseSymbol? Resolve(RefNode refNode)
    {
        return Resolve(refNode.TargetFullPath);
    }
}
