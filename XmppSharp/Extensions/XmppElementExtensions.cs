using System.Globalization;
using XmppSharp.Dom;

namespace XmppSharp.Extensions;

public static class XmppElementExtensions
{
    public delegate bool TryParseNumberDelegate<T>(
        string? s,
        NumberStyles style,
        IFormatProvider? ifp,
        out T result);

    public delegate (bool Success, T Result) TryParseNumberInplaceDelegate<T>(
        string? s,
        NumberStyles style,
        IFormatProvider? formatProvider);

    static TryParseNumberInplaceDelegate<T> CreateNumberParser<T>(TryParseNumberDelegate<T> func) => (value, style, ifp) =>
    {
        var result = func(value, style, ifp ?? CultureInfo.InvariantCulture, out var temp);
        return (result, temp);
    };

    public static TryParseNumberInplaceDelegate<byte> UInt8Parser { get; } = CreateNumberParser<byte>(byte.TryParse);
    public static TryParseNumberInplaceDelegate<sbyte> Int8Parser { get; } = CreateNumberParser<sbyte>(sbyte.TryParse);

    public static TryParseNumberInplaceDelegate<ushort> UInt16Parser { get; } = CreateNumberParser<ushort>(ushort.TryParse);
    public static TryParseNumberInplaceDelegate<short> Int16Parser { get; } = CreateNumberParser<short>(short.TryParse);

    public static TryParseNumberInplaceDelegate<uint> UInt32Parser { get; } = CreateNumberParser<uint>(uint.TryParse);
    public static TryParseNumberInplaceDelegate<int> Int32Parser { get; } = CreateNumberParser<int>(int.TryParse);

    public static TryParseNumberInplaceDelegate<ulong> UInt64Parser { get; } = CreateNumberParser<ulong>(ulong.TryParse);
    public static TryParseNumberInplaceDelegate<long> Int64Parser { get; } = CreateNumberParser<long>(long.TryParse);

    public static TryParseNumberInplaceDelegate<float> FloatParser { get; } = CreateNumberParser<float>(float.TryParse);
    public static TryParseNumberInplaceDelegate<double> DoubleParser { get; } = CreateNumberParser<double>(double.TryParse);

    public static bool GetAttributeBool(this XmppElement e, string name, bool defaultValue = false)
    {
        var temp = e.GetAttribute(name);

        if (temp == null)
            return defaultValue;

        return temp == "1" || temp.Equals("true", StringComparison.OrdinalIgnoreCase);
    }

    #region Signed Types

    public static sbyte GetAttributeInt8(this XmppElement e, string name, sbyte defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int8Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static short GetAttributeInt16(this XmppElement e, string name, short defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int16Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static int GetAttributeInt32(this XmppElement e, string name, int defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int32Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static long GetAttributeInt64(this XmppElement e, string name, long defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = Int64Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    #endregion

    #region Unsigned Types

    public static byte GetAttributeUInt8(this XmppElement e, string name, byte defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = UInt8Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static ushort GetAttributeUInt16(this XmppElement e, string name, ushort defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = UInt16Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static uint GetAttributeUInt32(this XmppElement e, string name, uint defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = UInt32Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static ulong GetAttributeUInt64(this XmppElement e, string name, ulong defaultValue = 0, NumberStyles style = NumberStyles.Integer, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = UInt64Parser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    #endregion

    #region Float Types

    public static float GetAttributeFloat(this XmppElement e, string name, float defaultValue = 0, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = FloatParser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    public static double GetAttributeDouble(this XmppElement e, string name, double defaultValue = 0, NumberStyles style = NumberStyles.Float, IFormatProvider? ifp = default)
    {
        var temp = e.Attributes.GetValueOrDefault(name);
        var (success, result) = DoubleParser(temp, style, ifp);
        return success ? result : defaultValue;
    }

    #endregion

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