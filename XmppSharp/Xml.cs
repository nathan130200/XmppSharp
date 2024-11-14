using System.Globalization;
using System.Security;
using System.Text;
using System.Web;
using System.Xml;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp;

public static class Xml
{

#if NET6_0

    public static XmlResolver ThrowingResolver { get; } = new ThrowingXmlResolverImpl();

    class ThrowingXmlResolverImpl : XmlResolver
    {
        public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            => throw new NotSupportedException();

        public override Task<object> GetEntityAsync(Uri absoluteUri, string? role, Type? ofObjectToReturn)
            => throw new NotSupportedException();
    }

#endif

    public static byte[] GetBytes(this string str, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(str);

    public static string GetString(this byte[] bytes, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(bytes);

    [ThreadStatic]
    private static string? s_IndentChars;

    [ThreadStatic]
    private static string? s_NewLineChars;

    public static string NewLineChars
    {
        get
        {
            s_NewLineChars ??= "\n";
            return s_NewLineChars;
        }
    }

    public static string IndentChars
    {
        get
        {
            s_IndentChars ??= "  ";
            return s_IndentChars;
        }
    }

    public static T AddError<T>(this T stanza, Action<StanzaError> callback) where T : Stanza
    {
        stanza.Error = new StanzaError();
        callback(stanza.Error);
        return stanza;
    }

    public static string? EncodeName(string? s)
        => XmlConvert.EncodeName(s);

    public static string? Escape(string? s)
        => SecurityElement.Escape(s);

    public static string? EscapeAttribute(string? s)
        => HttpUtility.HtmlAttributeEncode(s);

    public static string? Unescape(string? s)
        => HttpUtility.HtmlDecode(s);

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> callback)
    {
        ThrowHelper.ThrowIfNull(source);
        ThrowHelper.ThrowIfNull(callback);

        if (source.Any())
        {
            foreach (var item in source)
                callback(item);
        }

        return source;
    }

    public static XmlWriter CreateWriter(TextWriter textWriter, XmlFormatting formatting, Encoding? encoding = default)
    {
        var isFragment = formatting.HasFlag(XmlFormatting.OmitXmlDeclaration);

        return XmlWriter.Create(textWriter, new XmlWriterSettings()
        {
            Indent = formatting.HasFlag(XmlFormatting.Indented),
            IndentChars = IndentChars,
            ConformanceLevel = isFragment ? ConformanceLevel.Fragment : ConformanceLevel.Document,
            CloseOutput = false,
            Encoding = encoding ?? Encoding.UTF8,
            NamespaceHandling = formatting.HasFlag(XmlFormatting.OmitDuplicatedNamespaces)
                ? NamespaceHandling.OmitDuplicates
                : NamespaceHandling.Default,

            OmitXmlDeclaration = formatting.HasFlag(XmlFormatting.OmitXmlDeclaration),
            NewLineChars = NewLineChars,
            NewLineOnAttributes = formatting.HasFlag(XmlFormatting.NewLineOnAttributes),
            DoNotEscapeUriAttributes = formatting.HasFlag(XmlFormatting.DoNotEscapeUriAttributes),
            CheckCharacters = formatting.HasFlag(XmlFormatting.CheckCharacters),
        });
    }

    public static void Remove(this IEnumerable<Element> e)
    {
        if (e.Any())
        {
            foreach (var item in e)
                item.Remove();
        }
    }

    public static void WriteTree(Element e, XmlWriter xw)
    {
        var skipAttribute = e.Prefix == null ? "xmlns" : $"xmlns:{e.Prefix}";
        xw.WriteStartElement(e.Prefix, e.LocalName, e.Namespace);

        foreach (var (key, value) in e.Attributes)
        {
            if (key == skipAttribute)
                continue;

            var name = new XmppName(key);

            if (!name.HasPrefix)
                xw.WriteAttributeString(name.LocalName, value);
            else
            {
                xw.WriteAttributeString(name.LocalName, name.Prefix switch
                {
                    "xml" => Namespaces.Xml,
                    "xmlns" => Namespaces.Xmlns,
                    _ => e.GetNamespace(name.Prefix) ?? string.Empty
                }, value);
            }
        }

        foreach (var node in e.Nodes())
        {
            if (node is Element child)
                WriteTree(child, xw);
            else if (node is Text text)
                xw.WriteValue(text.Value);
            else if (node is Comment comment)
                xw.WriteComment(comment.Value);
            else if (node is Cdata cdata)
                xw.WriteCData(cdata.Value);
        }

        xw.WriteEndElement();
    }

    delegate bool TryParseDelegate<T>(string? s, out T result);
    delegate object? ValueConverterDelegate(string? s, IFormatProvider? formatProvider);

    static readonly Dictionary<Type, ValueConverterDelegate> s_ValueConverters = new()
    {
        [typeof(float)] = TryParseFloat,
        [typeof(double)] = TryParseDouble,

        [typeof(sbyte)] = CreateNumberParser<sbyte>(sbyte.TryParse),
        [typeof(short)] = CreateNumberParser<short>(short.TryParse),
        [typeof(int)] = CreateNumberParser<int>(int.TryParse),
        [typeof(long)] = CreateNumberParser<long>(long.TryParse),

        [typeof(byte)] = CreateNumberParser<byte>(byte.TryParse),
        [typeof(ushort)] = CreateNumberParser<ushort>(ushort.TryParse),
        [typeof(uint)] = CreateNumberParser<uint>(uint.TryParse),
        [typeof(ulong)] = CreateNumberParser<ulong>(ulong.TryParse),

        [typeof(bool)] = TryParseBool,
        [typeof(TimeSpan)] = TryParseTimeSpan,
        [typeof(DateTime)] = TryParseDateTime,
        [typeof(DateTimeOffset)] = TryParseDateTimeOffset,
    };

    static object? TryParseTimeSpan(string? s, IFormatProvider? ifp)
    {
        if (TimeSpan.TryParse(s, ifp, out var ts))
            return ts;

        return default;
    }

    static object? TryParseDateTimeOffset(string? s, IFormatProvider? ifp)
        => DateTimeOffset.TryParse(s, ifp, DateTimeStyles.None, out var res) ? res : default;

    static object? TryParseDateTime(string? s, IFormatProvider? ifp)
        => DateTime.TryParse(s, ifp, DateTimeStyles.None, out var res) ? res : default;

    static object? TryParseBool(string? s, IFormatProvider? _)
    {
        if (s == null)
            return null;

        return s == "1"
            || s.Equals("true", StringComparison.OrdinalIgnoreCase)
            || s.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase);
    }

    static object? TryParseFloat(string? s, IFormatProvider? ifp)
    {
        if (float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, ifp, out var result))
            return result;

        return default;
    }

    static object? TryParseDouble(string? s, IFormatProvider? ifp)
    {
        if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, ifp, out var result))
            return result;

        return default;
    }

    delegate bool TryParseNumberDelegate<T>(string? s, NumberStyles style, IFormatProvider? ifp, out T res);

    static ValueConverterDelegate CreateNumberParser<T>(TryParseNumberDelegate<T> cb)
    {
        return new((s, ifp) =>
        {
            if (cb(s, NumberStyles.Number, ifp ?? CultureInfo.InvariantCulture, out var result))
                return result;

            return default;
        });
    }

    public static T ParseValueOrDefault<T>(string? value, T defaultValue, IFormatProvider? ifp = default)
    {
        if (!s_ValueConverters.TryGetValue(typeof(T), out var conv))
            throw new NotImplementedException($"Parsing value of type '{typeof(T)}' is not implemented.");

        ifp ??= CultureInfo.InvariantCulture;

        var temp = conv!(value, ifp);

        if (temp == null)
            return defaultValue;

        return (T)temp;
    }

    public static T? ParseValue<T>(string? value, IFormatProvider? ifp = default) where T : struct
    {
        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (!s_ValueConverters.TryGetValue(type, out var conv))
            throw new NotImplementedException($"Parsing value of type '{typeof(T)}' is not implemented.");

        ifp ??= CultureInfo.InvariantCulture;

        var temp = conv!(value, ifp);

        if (temp == null)
            return default;

        return (T)temp;
    }

    public static T GetAttributeEnum<T>(this Element e, string name, T defaultValue, bool ignoreCase = true) where T : struct, Enum
    {
        var result = e.GetAttributeEnum<T>(name, ignoreCase);
        return result ?? defaultValue;
    }

    public static T? GetAttributeEnum<T>(this Element e, string name, bool ignoreCase = true) where T : struct, Enum
    {
        var str = e.GetAttribute(name);

        if (str != null)
        {
            {
                var temp = XmppEnum.FromXmlOrDefault<T>(str);

                if (temp.HasValue)
                    return temp.Value;
            }

            {
                if (Enum.TryParse<T>(str, ignoreCase, out var temp))
                    return temp;
            }
        }

        return null;
    }

#if NET6_0

    public static T GetAttribute<T>(this Element e, string name, T defaultValue)
        => ParseValueOrDefault(e.GetAttribute(name), defaultValue);

    public static T? GetAttribute<T>(this Element e, string name) where T : struct
        => ParseValue<T>(e.GetAttribute(name));

#elif NET7_0_OR_GREATER

    public static T GetAttribute<T>(this Element e, string name, T defaultValue, IFormatProvider? formatter = default) where T : IParsable<T>
    {
        formatter ??= CultureInfo.InvariantCulture;

        var attrVal = e.GetAttribute(name);

        if (!T.TryParse(attrVal, formatter, out var result))
            return defaultValue;

        return result;
    }

    public static T? GetAttribute<T>(this Element e, string name, IFormatProvider? formatter = default) where T : struct, IParsable<T>
    {
        formatter ??= CultureInfo.InvariantCulture;

        var attrVal = e.GetAttribute(name);

        if (!T.TryParse(attrVal, formatter, out var result))
            return default;

        return result;
    }

#endif

}
