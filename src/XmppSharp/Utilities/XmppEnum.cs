using System.Xml.Linq;
using XmppSharp.Protocol;

namespace XmppSharp.Utilities;

public static class XmppEnum
{
	public static XElement CreateElement(this StanzaErrorCondition condition, XNamespace xmlns,
		StanzaErrorType? type = default,
		string? message = default,
		string language = "en",
		XElement? child = default)
	{
		var result = new XElement(xmlns + "error");

		{
			if (!type.TryUnwrap(out var self))
				self = StanzaErrorType.Cancel;

			result.Add(new XAttribute("type", self.GetXmppEnumMember()!.Name));
		}

		result.C(Namespace.Stanzas.CreateElement(condition.GetTag()));

		if (!string.IsNullOrEmpty(message))
		{
			var text = Namespace.Stanzas.CreateElement("text");

			if (string.IsNullOrWhiteSpace(language))
				language = "en";

			text.SetAttributeValue(XNamespace.Xml + "lang", language);
		}

		if (child != null)
			result.Add(child);

		return result;
	}

	public static XElement CreateElement(this Namespace ns, string tag)
	{
		var uri = ns.Get();
		var prefix = ns.GetPrefix();

		var el = new XElement(uri + tag);

		if (prefix != null)
			el.Add(new XAttribute(XNamespace.Xmlns + prefix, uri));

		return el;
	}

	public static XNamespace Get(this Namespace xmlns)
		=> xmlns.GetXmppNamespace()!.Namespace;

	public static string? GetPrefix(this Namespace xmlns)
		=> xmlns.GetXmppNamespace()!.Prefix;

	public static string GetTag(this StreamErrorCondition condition)
		=> condition.GetXmppEnumMember()!.Name;

	public static string GetTag(this StanzaErrorCondition condition)
		=> condition.GetXmppEnumMember()!.Name;

	public static XElement CreateElement(this StreamErrorCondition condition,
		string? message = default,
		string lang = "en",
		XElement? child = default)
	{
		var error = Namespace.Stream.CreateElement("error");

		error.C(Namespace.Streams.CreateElement(condition.GetTag()));

		if (!string.IsNullOrWhiteSpace(message))
		{
			var text = error.C("text", Namespace.Stanzas.Get(), message);

			if (!string.IsNullOrWhiteSpace(lang))
				text.Add(new XAttribute(XNamespace.Xml + "lang", lang));
		}

		if (child != null)
			error.C(child);

		return error;
	}
}
