using System.Text;

namespace WillowGen.Renderers;

public static class PyIdentifier
{
    // shoutouts to that one twat who decided to name their parameter `del`
    private static readonly HashSet<string> Keywords = new (StringComparer.Ordinal)
    {
        "False",
        "None",
        "True",
        "and",
        "as",
        "assert",
        "async",
        "await",
        "break",
        "class",
        "continue",
        "def",
        "del",
        "elif",
        "else",
        "except",
        "finally",
        "for",
        "from",
        "global",
        "if",
        "import",
        "in",
        "is",
        "lambda",
        "nonlocal",
        "not",
        "or",
        "pass",
        "raise",
        "return",
        "try",
        "while",
        "with",
        "yield",
    };

    public static bool IsValid(string name)
    {
        if (name.Length == 0)
        {
            return false;
        }

        if (Keywords.Contains(name))
        {
            return false;
        }

        if (!(char.IsLetter(name[0]) || name[0] == '_'))
        {
            return false;
        }

        return name.All(c => char.IsLetterOrDigit(c) || c == '_');
    }

    public static string Sanitize(string name)
    {
        if (IsValid(name))
        {
            return name;
        }

        var sb = new StringBuilder(name.Length + 1);
        foreach (var c in name)
        {
            sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
        }

        if (sb.Length == 0 || char.IsDigit(sb[0]))
        {
            sb.Insert(0, '_');
        }

        var result = sb.ToString();
        if (Keywords.Contains(result))
        {
            result += "_";
        }

        return result;
    }
}
