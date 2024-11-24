using System.Globalization;
using System.Text;
using XmppSharp.Collections;
using XmppSharp.Dom;

namespace XmppSharp;

public static class Helpers
{
    public static byte[] GetBytes(this string str, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(str);

    public static string GetString(this byte[] bytes, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(bytes);

    public static byte[] GetBytes(this Node node)
        => node.ToString()!.GetBytes();

    public static bool IsBareEquals(this Jid? jid, Jid? other)
        => BareJidComparer.Shared.Compare(jid, other) == 0;

    public static bool IsFullEquals(this Jid? jid, Jid? other)
        => FullJidComparer.Shared.Compare(jid, other) == 0;

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

    public static bool? TryParseBool(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
            return default;

        if (s == "1" || s.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
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
                var temp = XmppEnum.FromXml<T>(str);

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
