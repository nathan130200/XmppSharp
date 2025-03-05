using System.Globalization;
using XmppSharp.Dom;

namespace XmppSharp.Extensions;

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

    public static TryParseNumberDelegate<sbyte> Int8Parser { get; } = CreateNumberParser<sbyte>(sbyte.TryParse);
    public static TryParseNumberDelegate<short> Int16Parser { get; } = CreateNumberParser<short>(short.TryParse);
    public static TryParseNumberDelegate<int> Int32Parser { get; } = CreateNumberParser<int>(int.TryParse);
    public static TryParseNumberDelegate<long> Int64Parser { get; } = CreateNumberParser<long>(long.TryParse);
    public static TryParseNumberDelegate<float> FloatParser { get; } = CreateNumberParser<float>(float.TryParse);
    public static TryParseNumberDelegate<double> DoubleParser { get; } = CreateNumberParser<double>(double.TryParse);

    public static bool? GetAttributeBool(this XmppElement e, string name)
    {
        var temp = e.GetAttribute(name);

        if (temp == null)
            return default;

        return temp == "1" || temp.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    public static sbyte? GetAttributeInt8(this XmppElement e, string name, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int8Parser(temp, style, ifp);
        return success ? result : default;
    }

    public static short? GetAttributeInt16(this XmppElement e, string name, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int16Parser(temp, style, ifp);
        return success ? result : default;
    }

    public static int? GetAttributeInt32(this XmppElement e, string name, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int32Parser(temp, style, ifp);
        return success ? result : default;
    }

    public static long? GetAttributeInt64(this XmppElement e, string name, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int64Parser(temp, style, ifp);
        return success ? result : default;
    }

    public static float? GetAttributeFloat(this XmppElement e, string name, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = FloatParser(temp, style, ifp);
        return success ? result : default;
    }

    public static double? GetAttributeDouble(this XmppElement e, string name, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = DoubleParser(temp, style, ifp);
        return success ? result : default;
    }

    public static Jid? GetAttributeJid(this XmppElement e, string name)
        => e.Attributes.GetValueOrDefault(name);

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

    public static T GetAttributeValue<T>(this XmppElement e, string name, T defaultValue, IFormatProvider? formatter = default) where T : IParsable<T>
    {
        formatter ??= CultureInfo.InvariantCulture;

        var attrVal = e.GetAttribute(name);

        if (!T.TryParse(attrVal, formatter, out var result))
            return defaultValue;

        return result;
    }

    public static T? GetAttributeValue<T>(this XmppElement e, string name, IFormatProvider? formatter = default) where T : struct, IParsable<T>
    {
        formatter ??= CultureInfo.InvariantCulture;

        var attrVal = e.GetAttribute(name);

        if (!T.TryParse(attrVal, formatter, out var result))
            return default;

        return result;
    }

#endif

}