using System.Globalization;
using XmppSharp.Dom;

namespace XmppSharp;

public static class XmppElementExtensions
{
    internal delegate bool OutTryParseNumberDelegate<T>(
        string? s,
        NumberStyles style,
        IFormatProvider? ifp,
        out T result);

    public delegate (bool Success, T Result) TryParseNumberDelegate<T>(
        string? s,
        NumberStyles style,
        IFormatProvider? formatProvider);

    static TryParseNumberDelegate<T> CreateNumberParser<T>(OutTryParseNumberDelegate<T> func)
        => (s, t, f) => (func(s, t, f, out var r), r);

    public static readonly TryParseNumberDelegate<short> Int16Parser = CreateNumberParser<short>(short.TryParse);
    public static readonly TryParseNumberDelegate<int> Int32Parser = CreateNumberParser<int>(int.TryParse);
    public static readonly TryParseNumberDelegate<long> Int64Parser = CreateNumberParser<long>(long.TryParse);
    public static readonly TryParseNumberDelegate<float> FloatParser = CreateNumberParser<float>(float.TryParse);
    public static readonly TryParseNumberDelegate<double> DoubleParser = CreateNumberParser<double>(double.TryParse);

    public static bool GetAttributeBool(this XmppElement e, string name, bool defaultValue = default)
    {
        var temp = e.GetAttribute(name);

        if (temp == null)
            return defaultValue;

        return temp == "1" || temp.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public static short GetAttributeInt16(this XmppElement e, string name, short defaultValue = default, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e._attributes.GetValueOrDefault(name);
        var (success, result) = Int16Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static int GetAttributeInt32(this XmppElement e, string name, int defaultValue = default, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e._attributes.GetValueOrDefault(name);
        var (success, result) = Int32Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static long GetAttributeInt64(this XmppElement e, string name, long defaultValue = default, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e._attributes.GetValueOrDefault(name);
        var (success, result) = Int64Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static float GetAttributeFloat(this XmppElement e, string name, float defaultValue = default, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e._attributes.GetValueOrDefault(name);
        var (success, result) = FloatParser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static double GetAttributeDouble(this XmppElement e, string name, double defaultValue = default, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e._attributes.GetValueOrDefault(name);
        var (success, result) = DoubleParser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static Jid? GetAttributeJid(this XmppElement e, string name, Jid? defaultValue = default)
    {
        var temp = e._attributes.GetValueOrDefault(name);
        return temp != null ? temp : defaultValue;
    }

    public static T GetAttributeEnum<T>(this XmppElement e, string name, T defaultValue, bool ignoreCase = true) where T : struct, Enum
    {
        var result = e.GetAttributeEnum<T>(name, ignoreCase);
        return result ?? defaultValue;
    }

    public static T? GetAttributeEnum<T>(this XmppElement e, string name, bool ignoreCase = true) where T : struct, Enum
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

    public static void SetAttributeEnum<T>(this XmppElement e, string name, T value) where T : struct, Enum
    {
        var temp = XmppEnum.ToXml(value);
        e.SetAttribute(name, temp != null ? temp : value);
    }

#if NET8_0_OR_GREATER

    public static T GetAttribute<T>(this XmppElement e, string name, T defaultValue, IFormatProvider? formatter = default) where T : IParsable<T>
    {
        formatter ??= CultureInfo.InvariantCulture;

        var attrVal = e.GetAttribute(name);

        if (!T.TryParse(attrVal, formatter, out var result))
            return defaultValue;

        return result;
    }

    public static T? GetAttribute<T>(this XmppElement e, string name, IFormatProvider? formatter = default) where T : struct, IParsable<T>
    {
        formatter ??= CultureInfo.InvariantCulture;

        var attrVal = e.GetAttribute(name);

        if (!T.TryParse(attrVal, formatter, out var result))
            return default;

        return result;
    }

#endif

}