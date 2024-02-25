using System.Globalization;
using System.Text;
using System.Xml;

namespace XmppSharp;

public static partial class Xml
{
    delegate bool TryParseDelegate<T>(string src, IFormatProvider fmt, out T result);
    delegate string ToStringDelegate<T>(T value);

    static T GetAttr<T>(this XmlElement e, string name, TryParseDelegate<T> tryParseFunc, T defaultValue)
    {
        var temp = e.GetAttribute(name);

        if (tryParseFunc(temp, CultureInfo.InvariantCulture, out var result))
            return result;

        return defaultValue;
    }

    static XmlElement SetAttr<T>(this XmlElement e, string name, T rawValue, string? format = default, IFormatProvider? formatProvider = default)
    {
        var value = string.Empty;

        formatProvider ??= CultureInfo.InvariantCulture;

        if (rawValue is not null)
        {
            if (rawValue is string s)
                value = s;
            else if (rawValue is IFormattable fmt)
                value = fmt.ToString(format, formatProvider);
            else if (rawValue is IConvertible conv)
                value = conv.ToString(formatProvider);
            else
                value = rawValue.ToString();
        }

        e.SetAttribute(name, value);

        return e;
    }

    public static int GetAttributeInt(this XmlElement e, string name, int defaultValue = 0)
        => e.GetAttr(name, int.TryParse, defaultValue);

    public static XmlElement SetAttributeInt(this XmlElement e, string name, int value, string? format = default, IFormatProvider? formatProvider = default)
        => e.SetAttr(name, value, format, formatProvider);

    public static long GetAttributeLong(this XmlElement e, string name, long defaultValue = 0)
        => e.GetAttr(name, long.TryParse, defaultValue);

    public static XmlElement SetAttributeLong(this XmlElement e, string name, long value, string? format = default, IFormatProvider? formatProvider = default)
        => e.SetAttr(name, value, format, formatProvider);

    public static float GetAttributeFloat(this XmlElement e, string name, float defaultValue = 0f)
        => e.GetAttr(name, float.TryParse, defaultValue);

    public static XmlElement SetAttributeFloat(this XmlElement e, string name, float value, string format = "F6", IFormatProvider? formatProvider = default)
        => e.SetAttr(name, value, format, formatProvider);

    public static double GetAttributeDouble(this XmlElement e, string name, double defaultValue = 0d)
        => e.GetAttr(name, double.TryParse, defaultValue);

    public static XmlElement SetAttributeDouble(this XmlElement e, string name, double value, string format = "F6", IFormatProvider? formatProvider = default)
        => e.SetAttr(name, value, format, formatProvider);

    public static Jid GetAttributeJid(this XmlElement e, string name)
        => Jid.Parse(e.GetAttribute(name));

    public static XmlElement SetAttributeJid(this XmlElement e, string name, Jid value)
    {
        e.SetAttribute(name, value.ToString());
        return e;
    }

    public static bool GetAttributeBool(this XmlElement e, string name, bool defaultValue = false)
    {
        var temp = e.GetAttribute(name);

        return temp.ToLowerInvariant() switch
        {
            BOOL_TRUE or BIT_TRUE => true,
            BOOL_FALSE or BIT_FALSE => false,
            _ => defaultValue
        };
    }

    const string BOOL_TRUE = "true";
    const string BOOL_FALSE = "false";
    const string BIT_TRUE = "1";
    const string BIT_FALSE = "0";

    public static XmlElement SetAttributeBool(this XmlElement e, string name, bool value, bool valueAsBit = true, IFormatProvider? formatProvider = default)
    {
        var tempValue = valueAsBit ? (value ? BIT_TRUE : BIT_FALSE) : (value ? BOOL_TRUE : BOOL_FALSE);
        e.SetAttribute(name, tempValue);
        return e;
    }

    public static Guid GetAttributeGuid(this XmlElement e, string name, Guid defaultValue = default)
        => e.GetAttr(name, Guid.TryParse, defaultValue);

    public static XmlElement SetAttributeGuid(this XmlElement e, string name, Guid value, string format = "D", IFormatProvider? formatProvider = default)
        => e.SetAttr(name, value, format, formatProvider);

    public static byte[] GetAttributeBase64(this XmlElement e, string name)
        => Convert.FromBase64String(e.GetAttribute(name));

    public static string GetAttributeBase64Text(this XmlElement e, string name, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(Convert.FromBase64String(e.GetAttribute(name)));

    public static XmlElement SetAttributeBase64(this XmlElement e, string name, byte[] value)
    {
        e.SetAttribute(name, Convert.ToBase64String(value));
        return e;
    }

    public static XmlElement SetAttributeBase64Text(this XmlElement e, string name, string value, Encoding? encoding = default)
    {
        e.SetAttribute(name, Convert.ToBase64String(value.GetBytes(encoding)));
        return e;
    }

    public static byte[] GetInnerTextAsBase64(this XmlElement e)
        => Convert.FromBase64String(e.InnerText);

    public static string GetInnerTextAsBase64String(this XmlElement e)
        => Encoding.UTF8.GetString(Convert.FromBase64String(e.InnerText));
}
