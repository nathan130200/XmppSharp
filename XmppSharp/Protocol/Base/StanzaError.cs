using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespaces.Client)]
[XmppTag("error", Namespaces.Server)]
[XmppTag("error", Namespaces.Accept)]
[XmppTag("error", Namespaces.Connect)]
public class StanzaError : Element
{
	public StanzaError() : base("error", Namespaces.Client)
	{

	}

	public StanzaErrorType? Type
	{
		get => XmppEnum.Parse<StanzaErrorType>(this.GetAttribute("type"));
		set
		{
			if (!value.TryGetValue(out var result))
				this.RemoveAttribute("type");
			else
				this.SetAttribute("type", result.ToXmppName());
		}
	}

	public StanzaErrorCondition? Condition
	{
		get
		{
			foreach (var (name, value) in XmppEnum.GetValues<StanzaErrorCondition>())
			{
				if (this.HasTag(name, Namespaces.Stanzas))
					return value;
			}

			return default;
		}
		set
		{
			XmppEnum.GetNames<StanzaErrorCondition>()
				.ForEach(name => this.RemoveTag(name, Namespaces.Stanzas));

			if (value.TryGetValue(out var result))
				this.SetTag(result.ToXmppName()!, Namespaces.Stanzas);
		}
	}


	// Legacy code API.
	public int? Code
	{
		get
		{
			var attr = GetAttribute("code");

			if (attr == null)
				return null;

			return int.Parse(attr);
		}
		set => this.SetAttribute("code", value);
	}

	// Legacy code API.
	public int? CustomCode
	{
		get
		{
			var attr = GetAttribute("custom_code");

			if (attr == null)
				return null;

			return int.Parse(attr);
		}
		set => this.SetAttribute("custom_code", value);
	}

	public string? Text
	{
		get => this.GetTag("text", Namespaces.Stanzas);
		set
		{
			if (value == null)
				this.RemoveTag("text", Namespaces.Stanzas);
			else
				this.SetTag("text", Namespaces.Stanzas, value);
		}
	}
}
