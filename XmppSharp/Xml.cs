using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace XmppSharp;

public readonly record struct XmlNameInfo(bool HasPrefix, string LocalName, string Prefix);

public static class Xml
{
    public const string StreamEnd = "</stream:stream>";

    public static XmlNameInfo ExtractQualifiedName(string s)
    {
        var ofs = s.IndexOf(':');
        string prefix = default, localName;

        if (ofs != -1)
            prefix = s[0..ofs];

        localName = s[(ofs + 1)..];

        return new XmlNameInfo(!string.IsNullOrWhiteSpace(prefix),
            localName, prefix);
    }

    public static string ToString(this XElement e, bool indented)
    {
        var sb = StringBuilderPool.Rent();

        using (var writer = CreateWriter(indented, sb))
            e.WriteTo(writer);

        return StringBuilderPool.Return(sb);
    }

    internal static XmlWriter CreateWriter(bool indented, StringBuilder output)
    {
        var settings = new XmlWriterSettings
        {
            Indent = indented,
            IndentChars = "  ",
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

    #region Get/Set/Remvove Tag (String)

    public static bool HasTag(this XElement e, string localName)
        => e.HasTag(e.GetNamespace() + localName);

    public static string GetTag(this XElement e, string localName)
        => e.GetTag(e.GetNamespace() + localName);

    public static void RemoveTag(this XElement e, string localName)
        => e.RemoveTag(e.GetNamespace() + localName);

    public static void SetTag(this XElement e, string localName, object value = default)
        => e.SetTag(e.GetNamespace() + localName, value);

    #endregion

    #region Get/Set/Remvove Tag (XName)

    public static bool HasTag(this XElement e, XName name)
        => e.Element(name) != null;

    public static string GetTag(this XElement e, XName name)
        => e.Element(name)?.Value;

    public static void RemoveTag(this XElement e, XName name)
        => e.Element(name)?.Remove();

    public static void SetTag(this XElement e, XName name, object value = default)
    {
        var result = new XElement(name);

        e.Add(result);

        if (value != null)
            result.SetValue(value);
    }

    #endregion


    #region Get/Set/Remove Attribute Helper

    public static string GetAttribute(this XElement e, XName name)
        => e.Attribute(name)?.Value;

    public static bool HasAttribute(this XElement e, XName name)
        => e.Attribute(name) != null;

    public static void RemoveAttribute(this XElement e, XName name)
        => e.Attribute(name)?.Remove();

    public static void SetAttribute(this XElement e, XName name, object value)
    {
        var attr = e.Attribute(name);

        if (value is null)
            attr?.Remove();
        else
        {
            if (attr == null)
                e.Add(new XAttribute(name, value));
            else
                attr.SetValue(value);
        }
    }

    #endregion

    #region Get/Set Namespace Helper

    public static XNamespace GetNamespace(this XElement e)
        => e.Name.Namespace;

    public static XNamespace GetNamespace(this XElement e, string prefix)
        => e.GetNamespaceOfPrefix(prefix);

    public static void SetNamespace(this XElement e, string xmlns)
    {
        var oldNamespace = e.Name.Namespace;

        e.Descendants().ForEach(n =>
        {
            if (n.Name.Namespace == oldNamespace)
                n.Name = XName.Get(n.Name.LocalName, xmlns);
        });
    }

    #endregion

    #region Get Element (String)

    public static XElement Child(this XElement e, string localName)
        => e.Element(e.GetNamespace() + localName);

    public static IEnumerable<XElement> Children(this XElement e, string localName)
        => e.Elements(e.GetNamespace() + localName);

    #endregion

    #region Get Element (Generic)    

    public static T Element<T>(this XElement e) where T : XElement
        => e.Elements<T>().FirstOrDefault();

    public static IEnumerable<T> Elements<T>(this XElement e) where T : XElement
        => e.Elements().OfType<T>();

    #endregion

    #region Try Get Element (String & XName)

    public static bool TryGetChild(this XElement e, string localName, out XElement result)
    {
        result = e.Element(e.GetNamespace() + localName);
        return result != null;
    }

    public static bool TryGetChild(this XElement e, XName name, out XElement result)
    {
        result = e.Element(name);
        return result != null;
    }

    #endregion
}