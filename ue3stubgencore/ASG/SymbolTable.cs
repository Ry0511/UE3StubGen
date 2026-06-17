namespace UE3StubGenCore.ASG;

public class SymbolTable
{
    private readonly Dictionary<string, BaseElement> _fullPathSymbols = new();

    public void Register(ISymbol sym)
    {
        if (sym is BaseElement elem)
        {
            _fullPathSymbols.Add(sym.ExportPathName(), elem);
        }
    }

    public BaseElement? Resolve(string fullPath) => _fullPathSymbols.GetValueOrDefault(fullPath);
    public BaseElement? Resolve(RefNode refNode) => Resolve(refNode.TargetFullPath);
}