using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using XmppSharp.Xmpp.Dom;

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
}
