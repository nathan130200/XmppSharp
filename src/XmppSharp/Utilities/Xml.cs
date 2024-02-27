using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;

namespace XmppSharp.Utilities;

public static class Xml
{
	/// <summary>
	/// Gets the qualified name of the element.
	/// </summary>
	/// <param name="element">Element from which the information will be obtained.</param>
	/// <returns>String containing qualified name of the XML tag.</returns>
	public static string TagName(this XElement element)
	{
		var name = element.Name;
		string? prefix = null;

		if (name.Namespace != null)
			prefix = element.GetPrefixOfNamespace(name.Namespace);

		return string.IsNullOrEmpty(prefix)
			? name.LocalName
			: string.Concat(prefix, ':', name.LocalName);
	}

	public static bool Is(this XElement element, XName name)
		=> element.Name == name;

	public static bool Is(this XElement element, string name, string? xmlns = default)
	{
		var uri = element.Name.NamespaceName;
		return element.Name.LocalName == name && (xmlns == null || uri == xmlns);
	}

	public static bool Is(this XElement element, string name, Namespace? ns)
	{
		var uri = element.Name.Namespace;
		var targetUri = ns?.Get() ?? null;
		return element.Name.LocalName == name && (targetUri == null || targetUri == uri);
	}


	public static XElement SwitchDirection(this XElement e)
	{
		var to = e.Attribute("to")?.Value;
		var from = e.Attribute("from")?.Value;
		e.RemoveAttribute(nameof(to));
		e.RemoveAttribute(nameof(to));

		if (to != null)
			e.SetAttribute(nameof(from), to);

		if (from != null)
			e.SetAttribute(nameof(to), from);

		return e;
	}

	public static XElement FirstChild(this XElement e)
		=> e.Descendants().FirstOrDefault();

	/// <summary>
	/// Gets the element's opening tag.
	/// </summary>
	/// <remarks>Very useful in the jabber protocol.</remarks>
	/// <param name="e">Element that will have the opening tag extracted.</param>
	/// <returns>String containing the XML with the start tag.</returns>
	public static string StartTag(this XElement e)
	{
		var sb = new StringBuilder($"<{e.TagName()}");

		foreach (var attr in e.Attributes())
			sb.Append($" {attr}");

		return sb.Append('>').ToString();
	}

	static bool ExtractName(string s, out string localName, [NotNullWhen(true)] out string? prefix)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(s);

		prefix = default;

		var ofs = s.IndexOf(':');

		if (ofs == -1)
		{
			localName = s;
			return false;
		}
		else
		{
			prefix = s[0..ofs];
			localName = s[(ofs + 1)..];
			return true;
		}
	}

	static XNamespace GetNamespaceOrParent(XNamespace? current, XNamespace? parent = default)
		=> (current ?? parent) ?? XNamespace.None;

	/// <summary>
	/// Creates a new loose element (that is, it does not have a document or is not attached to the XML tree).
	/// </summary>
	/// <remarks>
	/// This method is the initial entry point for building an XML tree as it has some helper functions that help manipulate the XML tree.
	/// </remarks>
	/// <param name="name">Qualified name of the XML tag.</param>
	/// <param name="ns">XML Namespace URI for the element.</param>
	/// <param name="text">Optional, defines the text content of the element</param>
	/// <returns>The instance of the created element.</returns>
	public static XElement Element(string name, XNamespace? ns, string? text = default)
	{
		ns ??= GetNamespaceOrParent(ns);

		var isQName = ExtractName(name, out name, out var prefix);
		var result = new XElement(ns + name);

		if (ns != null && isQName)
			result.Add(new XAttribute(XNamespace.Xmlns + prefix!, ns));

		if (text != null)
			result.Add(new XText(text));

		return result;
	}

	/// <summary>
	/// Adds a child element to the parent element with the provided information.
	/// </summary>
	/// <param name="parent">Element that will be changed.</param>
	/// <param name="name">Element tag name.</param>
	/// <param name="ns">Element namespace uri.</param>
	/// <param name="text">New text for the element.</param>
	/// <returns>The child element instance.</returns>
	public static XElement C(this XElement parent, string name, XNamespace? ns = default, string? text = default)
	{
		ns ??= GetNamespaceOrParent(ns, parent.GetDefaultNamespace());

		var child = Element(name, ns, text);
		parent.Add(child);
		return child;
	}

	/// <summary>
	/// Adds an existing child element to the parent element.
	/// </summary>
	/// <param name="parent">Instance of the parent node that will be added.</param>
	/// <param name="child">Child node instance already created</param>
	/// <returns>The parent element instance.</returns>
	public static XElement C(this XElement parent, XElement child)
	{
		parent.Add(child);
		return child;
	}

	/// <summary>
	/// Update the text content of the element.
	/// </summary>
	/// <param name="element">Element that will be changed.</param>
	/// <param name="value">New text for the element.</param>
	/// <param name="remove_nodes">If true, it will clear all child nodes of the element, keeping only the text. Otherwise it will add a text node to the end of the tree.</param>
	/// <returns>The current element instance.</returns>
	public static XElement Text(this XElement element, string? value, bool remove_nodes = true)
	{
		if (value != null)
		{
			if (remove_nodes)
				element.Value = value;
			else
			{
				if (element.LastNode is XText text)
					text.Value += value;
				else
					element.Add(new XText(value));
			}
		}

		return element;
	}

	/// <summary>
	/// Moves the reference of the element above it to which it belongs.
	/// </summary>
	/// <param name="child">Child element to which the tree belongs.</param>
	/// <returns>The parent element instance.</returns>
	public static XElement? Up(this XElement child)
	{
		if (child.Parent is XElement parent)
			return parent;

		return child;
	}

	/// <summary>
	/// Determines whether the element is the root or not of the XML tree.
	/// </summary>
	/// <param name="e">Child element to be queried about.</param>
	/// <returns>True if the given element is the root element. Otherwise false if it is not.</returns>
	public static bool IsRootElement(this XElement e)
		=> e.Parent == null;

	/// <summary>
	/// Traverses the XML tree and finds the root element.
	/// </summary>
	/// <param name="e">Child element to be queried about.</param>
	/// <returns>The instance of the found root element.</returns>
	public static XElement Root(this XElement child)
	{
		while (child.Parent != null)
			child = child.Parent;

		return child;
	}

	/// <summary>
	/// Determines whether the attribute exists on the element.
	/// </summary>
	/// <param name="name">Attribute name.</param>
	public static bool HasAttribute(this XElement e, XName name)
		=> e.Attribute(name) != null;

	/// <summary>
	/// Gets the content of the attribute on the element.
	/// </summary>
	/// <param name="name">Attribute name.</param>
	public static string? GetAttribute(this XElement e, XName name)
		=> e.Attribute(name)?.Value;

	/// <summary>
	/// Sets the content of the attribute on the element.
	/// </summary>
	/// <param name="name">Attribute name.</param>
	public static void SetAttribute(this XElement e, XName name, string value)
		=> e.SetAttributeValue(name, value);

	/// <summary>
	/// Removes the attribute from the element.
	/// </summary>
	/// <param name="name">Attribute name.</param>
	public static bool RemoveAttribute(this XElement e, XName name)
	{
		var attr = e.Attribute(name);
		attr?.Remove();
		return attr != null;
	}
}
