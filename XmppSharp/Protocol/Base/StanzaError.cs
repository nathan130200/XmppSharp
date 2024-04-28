using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[XmppTag("error", Namespace.Client)]
[XmppTag("error", Namespace.Server)]
[XmppTag("error", Namespace.Accept)]
[XmppTag("error", Namespace.Connect)]
public class StanzaError : Element
{
	public StanzaError() : base("error", Namespace.Client)
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
				if (this.HasTag(name, Namespace.Stanzas))
					return value;
			}

			return default;
		}
		set
		{
			XmppEnum.GetNames<StanzaErrorCondition>()
				.ForEach(name => this.RemoveTag(name, Namespace.Stanzas));

			if (value.TryGetValue(out var result))
				this.SetTag(result.ToXmppName(), Namespace.Stanzas);
		}
	}

	public string Text
	{
		get => this.GetTag("text", Namespace.Stanzas);
		set
		{
			if (value == null)
				this.RemoveTag("text", Namespace.Stanzas);
			else
				this.SetTag("text", Namespace.Stanzas, value);
		}
	}
}
