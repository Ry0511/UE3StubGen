namespace WillowGen.py;

public class SymbolTable
{
    private readonly Dictionary<string, ISymbol> _fullPathSymbols = new();

    public void Register(ISymbol sym)
    {
        if (sym.CanBeReferenced())
        {
            _fullPathSymbols[sym.ExportPathName()] = sym;
        }
    }

    public ISymbol? Resolve(string fullPath) => _fullPathSymbols.GetValueOrDefault(fullPath);
    public ISymbol? Resolve(PyRef @ref) => Resolve(@ref.FullPath);
}