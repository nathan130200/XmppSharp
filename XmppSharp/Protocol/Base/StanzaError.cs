using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

[Tag("error", Namespaces.Client)]
[Tag("error", Namespaces.Server)]
[Tag("error", Namespaces.Component)]
public class StanzaError : XmppElement
{
	public StanzaError() : base("error")
	{

	}

	public StanzaErrorType Type
	{
		get => XmppEnum.ParseOrDefault<StanzaErrorType>(GetAttribute("type"));
		set
		{
			if (!Enum.IsDefined(value))
				RemoveAttribute("type");
			else
				SetAttribute("type", value.ToXml());
		}
	}

	public string? By
	{
		get => GetAttribute("by");
		set => SetAttribute("by", value);
	}

	public StanzaErrorCondition Condition
	{
		get
		{
			foreach (var (name, value) in XmppEnum.GetMembers<StanzaErrorCondition>())
			{
				if (HasTag(name, Namespaces.Stanzas))
					return value;
			}

			return default;
		}
		set
		{
			foreach (var name in XmppEnum.GetNames<StanzaErrorCondition>())
				Element(name, Namespaces.Stanzas)?.Remove();

			if (Enum.IsDefined(value))
			{
				var name = XmppEnum.ToXml(value)!;
				SetTag(name, xmlns: Namespaces.Stanzas);
			}
		}
	}

	public int? Code
	{
		get => GetAttribute<int>("code");
		set
		{
			if (!value.HasValue)
				RemoveAttribute("code");
			else
				SetAttribute("code", (int)value);
		}
	}

	public int? CustomCode
	{
		get => GetAttribute<int>("custom_code");
		set
		{
			if (!value.HasValue)
				RemoveAttribute("custom_code");
			else
				SetAttribute("custom_code", (int)value);
		}
	}

	public string? Text
	{
		get => GetTag("text", Namespaces.Stanzas);
		set
		{
			RemoveTag("text", Namespaces.Stanzas);

			if (value != null)
				SetTag("text", Namespaces.Stanzas, value);
		}
	}
}
