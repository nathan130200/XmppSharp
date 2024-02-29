using System.Globalization;
using System.Reflection;
using XmppSharp.Xml.Dom;

namespace XmppSharp;

public static class Utilities
{
    public static bool ExtractQualifiedName(this string input, out string localName, out string prefix)
    {
        prefix = null;

        var ofs = input.IndexOf(':');

        if (ofs == -1)
        {
            localName = input;
            return false;
        }
        else
        {
            prefix = input[0..ofs];
            localName = input[(ofs + 1)..];
            return true;
        }
    }

    public static bool TryUnwrap<E>(this E? value, out E result) where E : struct
    {
        result = value.GetValueOrDefault();
        return value.HasValue;
    }

    public static Element Element(this Element parent, string name, string xmlns = default)
    {
        return parent.Elements().FirstOrDefault(x => x.Name == name
            && (xmlns == null || x.Namespace == xmlns));
    }

    public static Element SetTag(this Element parent, string name, string xmlns = default, string text = default)
    {
        string ns = default;

        if (xmlns != null)
        {
            if (xmlns != parent.Namespace)
                ns = xmlns;
        }

        var child = new Element(name, ns);

        if (text != null)
            child.Value = text;

        parent.AddChild(child);

        return child;
    }

    public static string GetTag(this Element parent, string name, string xmlns = default)
        => parent.Element(name, xmlns)?.Value;

    public static bool HasTag(this Element parent, string name, string xmlns = default)
        => parent.Element(name, xmlns) != null;

    public static Element GetRoot(this Element child)
    {
        while (!child.IsRoot)
            child = child.Parent;

        return child;
    }

    public static TValue GetAttributeValue<TValue>(this Element element, string name, TValue defaultValue = default, IFormatProvider ifp = default)
        where TValue : IParsable<TValue>
    {
        var attVal = element.GetAttribute(name);

        if (attVal != null)
        {
            if (TValue.TryParse(attVal, ifp ?? CultureInfo.InvariantCulture, out var result))
                return result;
        }

        return defaultValue;
    }

    public static void SetAttributeValue<TValue>(this Element element, string name, TValue value, string format = default, IFormatProvider ifp = default)
    {
        ifp ??= CultureInfo.InvariantCulture;

        string attVal;

        if (value is string s)
            attVal = s;
        else if (value is IFormattable fmt)
            attVal = fmt.ToString(format, ifp);
        else if (value is IConvertible conv)
            attVal = conv.ToString(ifp);
        else
            attVal = value.ToString();

        element.SetAttribute(name, attVal ?? string.Empty);
    }

    public static TEnum? GetAttributeEnum<TEnum>(this Element element, string name)
        where TEnum : struct, Enum
    {
        var attVal = element.GetAttribute(name);

        if (attVal != null)
            return XmppEnum.FromXml<TEnum>(attVal);

        return null;
    }

    public static TEnum GetAttributeEnum<TEnum>(this Element element, string name, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        var attVal = element.GetAttribute(name);

        if (attVal != null)
            return XmppEnum.FromXml(attVal, defaultValue);

        return defaultValue;
    }

    public static void SetAttributeEnum<TEnum>(this Element element, string name, TEnum value)
        where TEnum : struct, Enum
    {
        element.SetAttribute(name, XmppEnum.ToXml(value));
    }

    public static Element C(this Element parent, string name, string xmlns = default, string text = default, object attrs = default)
    {
        var el = parent.SetTag(name, xmlns, text);

        if (attrs != null)
        {
            foreach (var prop in attrs.GetType().GetTypeInfo().DeclaredProperties)
            {
                var attName = prop.Name;
                var attVal = SerializeXmlValue(prop.GetValue(attrs));
                el.SetAttribute(attName, attVal);
            }
        }

        return el;
    }

    const string DEFAULT_FLOATING_FORMAT = "G";

    static string SerializeXmlValue(object inputValue)
    {
        if (inputValue is null)
            return string.Empty;

        var baseType = inputValue.GetType();
        var underlyingType = Nullable.GetUnderlyingType(baseType);

        if (baseType.IsEnum || (underlyingType != null && underlyingType.IsEnum))
        {
            // unwrap Nullable<T> before
            if (!baseType.IsEnum)
                inputValue = GetNullableValueFromValueType(baseType = underlyingType, inputValue);

            var result = typeof(XmppEnum)
                .GetMethod("ToXml")
                .MakeGenericMethod(baseType)
                .Invoke(null, [inputValue]) as string;

            return result ?? (inputValue?.ToString() ?? string.Empty);
        }
        else if (underlyingType != null)
            inputValue = GetNullableValueFromValueType(underlyingType, inputValue);

        if (inputValue is null)
            return string.Empty;

        if (inputValue is IFormattable fmt)
        {
            if (inputValue is double d && double.IsNaN(d))
                return double.NaN.ToString();

            if (inputValue is float f && float.IsNaN(f))
                return float.NaN.ToString();

            if (inputValue is float or double or decimal)
                return fmt.ToString(DEFAULT_FLOATING_FORMAT, CultureInfo.InvariantCulture);

            return fmt.ToString(null, CultureInfo.InvariantCulture);
        }

        if (inputValue is string s)
            return s;

        if (inputValue is IConvertible conv)
            return conv.ToString(CultureInfo.InvariantCulture);

        return inputValue.ToString();
    }

    static object GetNullableValueFromValueType(Type type, object source)
    {
        return typeof(Nullable<>)
                .MakeGenericType(type)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "GetValueOrDefault" && x.GetParameters().Length == 0)
                .Invoke(source, []);
    }

    public static Element C(this Element parent, Element child)
    {
        parent.AddChild(child);
        return child;
    }

    public static Element Up(this Element parent)
        => parent.Parent ?? parent;
}