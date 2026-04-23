using System.Diagnostics.CodeAnalysis;
using System.Text;
using XmppSharp.Dom;
using XmppSharp.Dom.Abstractions;
using XmppSharp.Xml;

namespace XmppSharp;

/// <summary>
/// Provides utility methods for string manipulation.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Encodes the specified string into a sequence of bytes using the specified encoding. If no encoding is provided, <see cref="Encoding.UTF8" /> encoding will be used by default.
    /// </summary>
    /// <param name="s">The string to be encoded.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>A byte array containing the encoded string.</returns>
    public static byte[] GetBytes(this string s, Encoding? encoding = null)
        => (encoding ?? Encoding.UTF8).GetBytes(s);

    /// <summary>
    /// Decodes a sequence of bytes into a string using the specified encoding. If no encoding is provided, <see cref="Encoding.UTF8" /> encoding will be used by default.
    /// </summary>
    /// <param name="s">The byte array to be decoded.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The decoded string.</returns>
    public static string GetString(this byte[] s, Encoding? encoding = null)
        => (encoding ?? Encoding.UTF8).GetString(s);
}

/// <summary>
/// Provides utility methods for manipulating the Document Object Model (DOM) elements and nodes.
/// </summary>
public static class DomUtilities
{
    /// <summary>
    /// Removes each node in the source collection from its parent element.
    /// </summary>
    /// <typeparam name="T">
    /// The type of nodes in the source collection. This type must be a subclass of the <see cref="Node" />
    /// </typeparam>
    /// <param name="source">
    /// The collection of nodes to be removed. Each node in this collection will be removed from its parent element.
    /// </param>
    /// <returns>
    /// The original collection of nodes after they have been removed from their parent elements.
    /// </returns>
    [return: NotNullIfNotNull(nameof(source))]
    public static IEnumerable<T>? Remove<T>(this IEnumerable<T>? source) where T : Node
    {
        if (source != null)
        {
            foreach (var item in source)
                item.Remove();
        }

        return source;
    }

    /// <summary>
    /// Adds a child element with the specified tag name to the parent element and returns the parent element.
    /// </summary>
    /// <param name="parent">
    /// The parent element to which the child element will be added. The method will modify this element by adding a new child to it.
    /// </param>
    /// <param name="tagName">The tag name of the child element to be added.</param>
    /// <param name="editor">An optional action to further configure the child element.</param>
    /// <returns>The parent element after the child element has been added.</returns>
    public static Element C(this Element parent, string tagName, Action<Element>? editor = null)
    {
        var child = ElementFactory.Create(tagName, default);

        parent.AddChild(child);

        editor?.Invoke(child);

        return parent;
    }

    /// <summary>
    /// Adds a child element with the specified tag name to the parent element and returns the parent element.
    /// </summary>
    /// <param name="parent">
    /// The parent element to which the child element will be added. The method will modify this element by adding a new child to it.
    /// </param>
    /// <param name="tagName">The tag name of the child element to be added.</param>
    /// <param name="namespaceUri">The namespace URI of the child element to be added. This can be null if the child element does not belong to any namespace.</param>
    /// <param name="editor">An optional action to further configure the child element.</param>
    /// <returns>The parent element after the child element has been added.</returns>
    public static Element C(this Element parent, string tagName, string? namespaceUri, Action<Element>? editor = null)
    {
        var child = ElementFactory.Create(tagName, namespaceUri);

        parent.AddChild(child);

        editor?.Invoke(child);

        return parent;
    }

    /// <summary>
    /// Returns the parent element of the specified child element. If the child element does not have a parent, it returns the child element itself.
    /// </summary>
    /// <param name="child">The child element whose parent is to be returned.</param>
    /// <returns>The parent element of the specified child element, or the child element itself if it has no parent.</returns>
    public static Element Up(this Element child)
        => child.Parent ?? child;

    /// <summary>
    /// Returns the root element of the specified child element by traversing up the parent hierarchy.
    /// </summary>
    /// <param name="child">
    /// The child element whose root element is to be returned or the child element itself if it has no parent.
    /// </param>
    /// <returns>The root element of the specified child element.</returns>
    public static Element Root(this Element child)
    {
        while (child.Parent != null)
            child = child.Parent;

        return child;
    }
}