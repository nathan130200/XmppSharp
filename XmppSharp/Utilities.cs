using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Utilities
{
    public static bool TryGetValue<T>(this T? self, out T result) where T : struct
    {
        result = self ?? default;
        return self.HasValue;
    }

    public static void Remove(this IEnumerable<Element> elements)
    {
        if (elements.Any())
        {
            foreach (var element in elements)
                element.Remove();
        }
    }

    public static string GetString(this byte[] buffer, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(buffer);

    public static byte[] GetBytes(this string s, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(s);

    public static string GetHex(this byte[] buffer, bool lowercase = true)
    {
        var result = Convert.ToHexString(buffer);

        if (lowercase)
            return result.ToLowerInvariant();

        return result;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> callback)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        ArgumentNullException.ThrowIfNull(callback);

        if (enumerable.Any())
        {
            foreach (var item in enumerable)
                callback(item);
        }

        return enumerable;
    }

#if NET7_0_OR_GREATER

    public static Element GetAttr<T>(this Element e, string name, out T result, IFormatProvider provider = default)
        where T : IParsable<T>
    {
        result = default;

        if (e.HasAttribute(name))
            T.TryParse(e.GetAttribute(name), provider ?? CultureInfo.InvariantCulture, out result);

        return e;
    }

    public static T GetAttr<T>(this Element e, string name, T defaultValue = default, IFormatProvider provider = default) where T : IParsable<T>
    {
        Unsafe.SkipInit(out T result);

        if (e.HasAttribute(name))
            if (!T.TryParse(e.GetAttribute(name), provider ?? CultureInfo.InvariantCulture, out result))
                result = defaultValue;

        return result;
    }

#endif

    public static Element SetAttr<T>(this Element e, string name, T rawValue, string format = default, IFormatProvider provider = default)
    {
        provider ??= CultureInfo.InvariantCulture;

        string value;

        if (rawValue is null)
            value = string.Empty;
        else if (rawValue is string s)
            value = s;
        else if (rawValue is IFormattable fmt)
            value = fmt.ToString(format, provider);
        else if (rawValue is IConvertible conv)
            value = conv.ToString(provider);
        else
            value = rawValue.ToString();

        e.SetAttribute(name, value);

        return e;
    }

    public static Element C(this Element e, string name, string xmlns = default, string value = default)
    {
        var child = new Element(name, xmlns, value);
        e.AddChild(child);
        return child;
    }

    public static Element Up(this Element e)
        => e.Parent;

    public static Element Root(this Element e)
    {
        while (!e.IsRootElement)
            e = e.Parent;

        return e;
    }

    public static Element Attr(this Element e, string name, object rawValue = default, string format = default, IFormatProvider provider = default)
    {
        provider ??= CultureInfo.InvariantCulture;
        e.SetAttribute(name, ToStringValue(rawValue, format, provider));
        return e;
    }

    static string ToStringValue(object rawValue, string format, IFormatProvider provider)
    {
        string value;

        if (rawValue is null)
            value = string.Empty;
        else if (rawValue is string s)
            value = s;
        else if (rawValue is IFormattable fmt)
            value = fmt.ToString(format, provider);
        else if (rawValue is IConvertible conv)
            value = conv.ToString(provider);
        else
            value = rawValue.ToString();

        return value;
    }

    public static Element Attrs(this Element e, params ITuple[] entries)
    {
        if (entries?.Any() == true)
        {
            foreach (var entry in entries)
            {
                var attrName = (string)entry[0];
                var rawValue = entry[1];

                string format = default;
                IFormatProvider provider = null;

                if (entry.Length > 2)
                    format = entry[2] as string;

                if (entry.Length > 3)
                    provider = entry[3] as IFormatProvider;

                e.SetAttr(attrName, ToStringValue(rawValue, format, provider));
            }
        }

        return e;
    }
}
