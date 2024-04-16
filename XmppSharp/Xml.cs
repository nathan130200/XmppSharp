using System.Text;
using System.Xml;

namespace XmppSharp;

public readonly record struct XmlQualifiedName
{
    public bool HasPrefix => !string.IsNullOrWhiteSpace(Prefix);
    public string LocalName { get; init; }
    public string? Prefix { get; init; }
}

public static class Xml
{
    public const string XmppStreamEnd = "</stream:stream>";

    public static XmlQualifiedName ExtractQualifiedName(string source)
    {
        Require.NotNullOrWhiteSpace(source);

        var ofs = source.IndexOf(':');

        string prefix = default;

        if (ofs != -1)
            prefix = source[0..ofs];

        var localName = source[(ofs + 1)..];

        return new()
        {
            LocalName = localName,
            Prefix = prefix
        };
    }

    public static string ToString(this Element element, bool indented, char indentChar = ' ', int indentSize = 2)
    {
        Require.NotNull(element);

        using (StringBuilderPool.Rent(out var sb))
        {
            using (var writer = CreateWriter(indented, sb, indentChar, indentSize))
                element.WriteTo(writer);

            return sb.ToString();
        }
    }

    internal static XmlWriter CreateWriter(bool indented, StringBuilder output, char indentChar, int indentSize)
    {
        Require.NotNull(output);

        var settings = new XmlWriterSettings
        {
            Indent = indented,
            IndentChars = new(indentChar, indentSize),
            CheckCharacters = true,
            CloseOutput = true,
            ConformanceLevel = ConformanceLevel.Fragment,
            Encoding = Encoding.UTF8,
            NamespaceHandling = NamespaceHandling.OmitDuplicates,
            OmitXmlDeclaration = true,
            NewLineChars = "\n"
        };

        return XmlWriter.Create(new StringWriter(output), settings);
    }

    public static void Remove<T>(this IEnumerable<T> source) where T : Node
    {
        if (source is IList<T> list)
        {
            foreach (var item in list)
                item.Remove();
        }
        else
        {
            foreach (var item in source)
                item.Remove();
        }
    }
}