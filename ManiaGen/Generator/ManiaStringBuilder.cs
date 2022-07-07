using System.Text;

namespace ManiaGen.Generator;

public class ManiaStringBuilder
{
    public readonly StringBuilder StringBuilder = new(4096);

    private int _scopeLevel = 0;

    public bool Compact { get; set; } = false;
    public bool EnableUserComments { get; set; } = false;
    public bool EnableVerboseComments { get; set; } = false;

    public void BeginBracket()
    {
        // This will mostly happen when in Compact, the ManiaScript parser will think that {{{ is an error
        if (StringBuilder[^1] == '{') StringBuilder.Append(' ');
        StringBuilder.Append('{');
        BeginScope();
    }

    public void EndBracket()
    {
        EndScope();
        AppendLine();
        // This will mostly happen when in Compact, the ManiaScript parser will think that }}} is an error
        if (StringBuilder[^1] == '}') StringBuilder.Append(' ');
        StringBuilder.Append('}');
    }
    
    public void BeginScope()
    {
        _scopeLevel++;
    }

    private static string GetLinePrefixCache(int level)
    {
        return $"\n{string.Join(null, Enumerable.Repeat("    ", level))}";
    }

    private static string[] _cachedLinePrefix = Enumerable
        .Range(0, 8)
        .Select(i =>
        {
            return GetLinePrefixCache(i);
        }).ToArray();

    public string GetLinePrefix()
    {
        if (Compact) return string.Empty;
        if (_cachedLinePrefix.Length < _scopeLevel)
           return $"\n{string.Join(null, Enumerable.Repeat("    ", _scopeLevel))}";
        return _cachedLinePrefix[_scopeLevel];
    }

    public void AppendLine()
    {
        StringBuilder.Append(GetLinePrefix());
    }
    
    public void AppendLine(string text)
    {
        StringBuilder.Append(GetLinePrefix());
        StringBuilder.Append(text);
    }

    public void EndScope()
    {
        _scopeLevel--;
    }

    public override string ToString()
    {
        return StringBuilder.ToString();
    }

    public void AppendXml(string name, string value)
    {
        StringBuilder.Append(' ');
        StringBuilder.Append(name);
        StringBuilder.Append('=');
        StringBuilder.Append('"');
        StringBuilder.Append(value);
        StringBuilder.Append('"');
    }

    public void VerboseComment(string s, string type = "INFO")
    {
        if (!EnableVerboseComments)
            return;
        
        StringBuilder.Append("/* ");
        StringBuilder.Append(type);
        StringBuilder.Append(" : ");
        StringBuilder.Append(s);
        StringBuilder.Append(" */");
        AppendLine();
    }
}