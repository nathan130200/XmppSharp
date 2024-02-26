using System.Xml.Linq;

namespace XmppSharp.Utilities;

public static class Xml
{
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

	public static string StartTag(this XElement e)
	{
		var sb = StringBuilderPool.Rent($"<{e.TagName()}");

		try
		{
			foreach (var attr in e.Attributes())
				sb.Append($" {attr}");

			return sb.Append('>').ToString();
		}
		finally
		{
			StringBuilderPool.Return(sb);
		}
	}

	static XName GetName(string tag, string? innerNamespace = default, XNamespace? outerNamespace = default)
	{
		XNamespace ns = (innerNamespace ?? outerNamespace) ?? XNamespace.None;
		return ns + tag;
	}

	public static XElement Element(string name, string? xmlns, string? text = default)
	{
		var ofs = name.IndexOf(':');

		var result = new XElement(GetName(name, xmlns));

		if (ofs != -1)
		{
			var prefix = name[0..ofs];
			result.Add(new XAttribute(XNamespace.Xml + prefix, xmlns!));
		}

		if (!string.IsNullOrWhiteSpace(text))
			result.Value = text;

		return result;
	}

	public static XElement C(this XElement parent, string name, string? xmlns = default, string? text = default)
	{
		var child = new XElement(GetName(name, xmlns, parent.GetDefaultNamespace()));

		if (!string.IsNullOrWhiteSpace(text))
			child.Value = text;

		return child;
	}

	public static XElement C(this XElement parent, XElement child)
	{
		parent.Add(child);
		return child;
	}

	public static XElement Text(this XElement element, string? value)
	{
		if (value != null)
			element.Value = value;

		return element;
	}

	public static XElement? Up(this XElement child)
	{
		if (child.Parent is XElement parent)
			return parent;

		return child;
	}

	public static bool IsRootElement(this XElement e)
		=> e.Parent == null;

	public static XElement Root(this XElement child)
	{
		while (child.Parent != null)
			child = child.Parent;

		return child;
	}

	public static bool HasAttribute(this XElement e, XName name)
		=> e.Attribute(name) != null;

	public static string? GetAttribute(this XElement e, XName name)
		=> e.Attribute(name)?.Value;

	public static void SetAttribute(this XElement e, XName name, string value)
		=> e.SetAttributeValue(name, value);

	public static bool RemoveAttribute(this XElement e, XName name)
	{
		var attr = e.Attribute(name);
		attr?.Remove();
		return attr != null;
	}
}
