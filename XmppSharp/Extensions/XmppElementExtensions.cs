using System.Globalization;
using XmppSharp.Dom;

namespace XmppSharp.Extensions;

public static class XmppElementExtensions
{
    public static T? GetAttribute<T>(this XmppElement e, string name, IFormatProvider? ifp = default)
        where T : struct, IParsable<T>
    {
        if (e.HasAttribute(name))
        {
            if (T.TryParse(e.GetAttribute(name), ifp ?? CultureInfo.InvariantCulture, out var result))
                return result;
        }

        return default;
    }

    public static T GetAttribute<T>(this XmppElement e, string name, T defaultValue, IFormatProvider? ifp = default)
        where T : IParsable<T>
    {
        if (e.HasAttribute(name))
        {
            if (T.TryParse(e.GetAttribute(name), ifp ?? CultureInfo.InvariantCulture, out var result))
                return result;
        }

        return defaultValue;
    }

    public static bool? GetAttributeBool(this XmppElement e, string name)
    {
        if (!e.HasAttribute(name))
            return null;

        var temp = e.GetAttribute(name, string.Empty)!;

        if (!bool.TryParse(temp, out var result))
        {
            unchecked
            {
                if (int.TryParse(temp, out var num))
                    return num > 0;
            }

            return null;
        }

        return result;
    }

    public static bool GetAttributeBool(this XmppElement e, string name, bool defaultValue)
        => e.GetAttributeBool(name) ?? defaultValue;

    public static T GetAttributeEnum<T>(this XmppElement e, string name, T defaultValue, bool ignoreCase = true) where T : struct, Enum
        => e.GetAttributeEnum<T>(name, ignoreCase) ?? defaultValue;

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
}