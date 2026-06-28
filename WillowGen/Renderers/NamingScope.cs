using UE3StubGenCore.ASG;

namespace WillowGen.Renderers;

// fullPath -> local name
public sealed class NamingScope(IReadOnlyDictionary<string, string> aliases)
{
    public static readonly NamingScope Empty = new(new Dictionary<string, string>());

    public string LocalName(BaseSymbol sym, string fallback)
    {
        return aliases.GetValueOrDefault(sym.ExportPathName(), fallback);
    }
}
