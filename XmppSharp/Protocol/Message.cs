using System.Globalization;
using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0085;

namespace XmppSharp.Protocol;

[Tag("message", Namespaces.Client)]
[Tag("message", Namespaces.Server)]
[Tag("message", Namespaces.Component)]
public class Message : Stanza
{
	public Message() : base("message")
	{

	}

	public new MessageType Type
	{
		get => XmppEnum.ParseOrDefault(base.Type, MessageType.Normal);
		set => base.Type = value.ToXmlOrDefault();
	}

	public ChatStates ChatState
	{
		get
		{
			foreach (var (key, value) in XmppEnum.GetMembers<ChatStates>())
			{
				if (HasTag(key, Namespaces.ChatStates))
					return value;
			}

			return default;
		}
		set
		{
			Elements(e => e.Namespace == Namespaces.ChatStates).Remove();

			if (Enum.IsDefined(value))
				SetTag(value.ToXml(), Namespaces.ChatStates);
		}
	}

	public string? Body
	{
		get => GetContent();
		set
		{
			RemoveTag("body");

			if (value != null)
				SetTag("body", value: value);
		}
	}

	public string? GetContent(CultureInfo? culture = null)
	{
		var isInvariantCulture = culture == null || culture.IsNeutralCulture;

		var element = Element(x =>
		{
			if (x.TagName == "body")
			{
				if (isInvariantCulture && !x.HasAttribute("xml:lang"))
					return true;

				if (!isInvariantCulture && x.GetAttribute("xml:lang")?.Equals(culture!.Name, StringComparison.OrdinalIgnoreCase) == true)
					return true;
			}

			return false;
		});

		return element?.InnerText;
	}
}