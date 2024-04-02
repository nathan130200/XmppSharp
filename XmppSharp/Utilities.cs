using System.Globalization;
using System.Text;
using XmppSharp.Dom;

namespace XmppSharp;

/// <summary>
/// Class that contains a series of helper functions in the library.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Helper method to wrap <see cref="Jid" /> into <see cref="ReadOnlyJid" /> without using the implicit operator and cloning it.
    /// </summary>
    /// <param name="jid">JID that will be used.</param>
    /// <returns><see cref="ReadOnlyJid" /> instance wrapping the given <paramref name="jid"/> instance.</returns>
    public static ReadOnlyJid AsReadOnly(this Jid jid)
        => new(jid);

    /// <summary>
    /// Try to unwrap the value of <see cref="Nullable{T}" /> in the form of inlining.
    /// </summary>
    /// <typeparam name="T">The underlying value type of the <see cref="Nullable{T}"/> type.</typeparam>
    /// <param name="self">Nullable instance value.</param>
    /// <param name="result">Output variable to receive the value if it is not null (i.e.: <see cref="Nullable{T}.HasValue" /> is <see langword="true" />)</param>
    /// <returns>Returns the same value as <see cref="Nullable{T}.HasValue" />.</returns>
    public static bool TryUnwrap<T>(this T? self, out T result) where T : struct
    {
        result = self ?? default;
        return self.HasValue;
    }

    /// <summary>
    /// Removes all elements from the collection from their respective parents.
    /// </summary>
    /// <param name="elements">Elements that will be removed.</param>
    public static void Remove(this IEnumerable<Element> elements)
        => elements.ForEach(n => n.Remove());

    /// <summary>
    /// Converts the current byte array to a string with a specific <paramref name="encoding" />.
    /// </summary>
    /// <param name="buffer">Bytes that will be converted to string.</param>
    /// <param name="encoding">Specify encoding for conversion. (Default: <see cref="Encoding.UTF8" />)</param>
    /// <returns>String converted from bytes.</returns>
    public static string GetString(this byte[] buffer, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetString(buffer);

    /// <summary>
    /// Converts the current string to a byte array with a specific <paramref name="encoding" />.
    /// </summary>
    /// <param name="s">String that will be converted to byte array.</param>
    /// <param name="encoding">Specify encoding for conversion. (Default: <see cref="Encoding.UTF8" />)</param>
    /// <returns>Bytes converted from string.</returns>
    public static byte[] GetBytes(this string s, Encoding? encoding = default)
        => (encoding ?? Encoding.UTF8).GetBytes(s);

    /// <summary>
    /// Transform bytes to hexadecimal string.
    /// </summary>
    /// <param name="bytes">Bytes that will be transformed into a hexadecimal string.</param>
    /// <param name="lowercase">Controls whether characters will be in lower case or not.</param>
    /// <returns>Hexadecimal string representation of bytes.</returns>
    public static string ToHex(this byte[] bytes, bool lowercase = true)
    {
        var result = Convert.ToHexString(bytes);

        if (!lowercase)
            return result;

        return result.ToLowerInvariant();
    }

    /// <summary>
    /// Transform hexadecimal string to bytes.
    /// </summary>
    /// <param name="str">Hexadecimal string that will be transformed back into bytes.</param>
    /// <returns>Bytes representation of Hexadecimal string.</returns>
    public static byte[] FromHex(this string str)
        => Convert.FromHexString(str);

    /// <summary>
    /// Auxiliary function that loops through the sequence of <typeparamref name="T" /> elements with a callback function that will be executed on each element in the collection.
    /// </summary>
    /// <typeparam name="T">Collection type.</typeparam>
    /// <param name="collection">Collection elements.</param>
    /// <param name="callback">Callback function that will be executed on each element.</param>
    /// <returns>The collection instance itself for nesting other LINQ methods.</returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> callback)
    {
        Require.NotNull(collection);
        Require.NotNull(callback);

        if (collection.Any())
        {
            foreach (var item in collection)
                callback(item);
        }

        return collection;
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Gets the value of the parsed attribute as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Type attribute value.</typeparam>
    /// <param name="e">Instance of the current element.</param>
    /// <param name="name">Attribute name.</param>
    /// <param name="defaultValue">A fallback value if the attribute does not exist or cannot be parsed.</param>
    /// <param name="provider">Optionally the format provider that will be used for parsing.</param>
    /// <returns>Attribute value parsed as <typeparamref name="T" />.</returns>
    public static T GetAttributeValue<T>(this Element e, string name, T defaultValue = default, IFormatProvider provider = default) where T : IParsable<T>
    {
        if (e.HasAttribute(name) && T.TryParse(e.GetAttribute(name), provider ?? CultureInfo.InvariantCulture, out var result))
            return result;

        return defaultValue;
    }
#endif

    /// <summary>
    /// Gets the value of the parsed attribute as <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e">Instance of the current element.</param>
    /// <param name="name">Attribute name.</param>
    /// <param name="converter">Function that will convert the string into <typeparamref name="T" />.</param>
    /// <param name="defaultValue">A fallback value if the attribute does not exist or cannot be parsed.</param>
    /// <returns>Attribute value parsed as <typeparamref name="T" />.</returns>
    public static T GetAttributeValue<T>(this Element e, string name, TryParseDelegate<T> converter, T defaultValue = default)
    {
        if (e.HasAttribute(name))
        {
            var val = e.GetAttribute(name);

            if (val != null && converter(val, out T result))
                return result;
        }

        return defaultValue;
    }

    /// <summary>
    /// Sets the attribute value.
    /// </summary>
    /// <typeparam name="E">Element type.</typeparam>
    /// <typeparam name="V">Value type.</typeparam>
    /// <param name="e">Instance of the current element.</param>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Nullable value of the attribute that will be converted into a string.</param>
    /// <param name="format">Optional format used for conversion to string.</param>
    /// <param name="ifp">Optionally the format provider that will be used in conversion.</param>
    /// <returns>Instance of the element itself for nesting other functions.</returns>
    public static E SetAttributeValue<E, V>(this E e, string name, V? value, string? format = default, IFormatProvider? ifp = default)
        where E : Element
        where V : struct
    {
        return e.SetAttributeValue(name, value.GetValueOrDefault(), format, ifp);
    }

    /// <summary>
    /// Sets the attribute value.
    /// </summary>
    /// <typeparam name="E">Element type.</typeparam>
    /// <param name="e">Instance of the current element.</param>
    /// <param name="name">Attribute name.</param>
    /// <param name="value">Raw value of the attribute that will be converted into a string.</param>
    /// <param name="format">Optional format used for conversion to string.</param>
    /// <param name="ifp">Optionally the format provider that will be used in conversion.</param>
    /// <returns>Instance of the element itself for nesting other functions.</returns>
    public static E SetAttributeValue<E>(this E e, string name, object value, string? format = default, IFormatProvider? ifp = default)
        where E : Element
    {
        ifp ??= CultureInfo.InvariantCulture;

        if (value is IFormattable fmt)
            e.SetAttribute(name, fmt.ToString(format, ifp));
        else if (value is IConvertible conv)
            e.SetAttribute(name, conv.ToString(ifp));
        else
            e.SetAttribute(name, value?.ToString() ?? string.Empty);

        return e;
    }

    /// <summary>
    /// Creates a child element.
    /// </summary>
    /// <param name="e">Instance of the current element.</param>
    /// <param name="name">Qualified name.</param>
    /// <param name="xmlns">
    /// Child element namespace.
    /// <para>
    /// If not provided, the child element will inherit the namespace of the parent element.
    /// </para>
    /// </param>
    /// <param name="value">Content (text) of the child element.</param>
    /// <returns>Instance of the child element for nesting other functions.</returns>
    public static Element C(this Element e, string name, string xmlns = default, string value = default)
    {
        if (xmlns == null)
        {
            var qualifiedName = Xml.ExtractQualifiedName(name);

            xmlns = qualifiedName.HasPrefix
                ? e.GetNamespace(qualifiedName.Prefix)
                : e.Namespace;
        }

        var child = new Element(name, xmlns, value);
        e.AddChild(child);
        return child;
    }

    /// <summary>
    /// Adds a child element to the current parent and returns the parent.
    /// </summary>
    /// <typeparam name="E">Element type.</typeparam>
    /// <param name="e">Parent element.</param>
    /// <param name="child">Child element.</param>
    /// <returns>Instance of the parent element for nesting other functions.</returns>
    public static E C<E>(this E e, Element child) where E : Element
    {
        e.AddChild(child);
        return e;
    }

    /// <summary>
    /// Helper function to get the parent element.
    /// </summary>
    /// <param name="e">Child element that will be used as a starting point.</param>|
    /// <returns>The parent element or <see langword="null"/> if the given element is the root element of the XML tree.</returns>
    public static Element Up(this Element e)
        => e.Parent;

    /// <summary>
    /// Helper function to get the root element of the current XML tree.
    /// </summary>
    /// <param name="e">Child element that will be used as a starting point.</param>|
    /// <returns>Root element or the element itself if it is the root of the XML tree.</returns>
    public static Element Root(this Element e)
    {
        while (!e.IsRootElement)
            e = e.Parent;

        return e;
    }
}
