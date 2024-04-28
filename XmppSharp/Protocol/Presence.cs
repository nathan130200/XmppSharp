using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("presence", Namespace.Client)]
[XmppTag("presence", Namespace.Server)]
[XmppTag("presence", Namespace.Accept)]
[XmppTag("presence", Namespace.Connect)]
public class Presence : Stanza
{
	public Presence() : base("presence", Namespace.Client)
	{

	}

	public Presence(PresenceType type) : this()
		=> this.Type = type;

	public new PresenceType Type
	{
		get => XmppEnum.ParseOrDefault(base.Type, PresenceType.Available);
		set
		{
			if (value == PresenceType.Available)
				base.Type = null;
			else
				base.Type = value.ToXmppName();
		}
	}

	public sbyte Priority
	{
		get
		{
			var value = this.GetTag("priority");

			if (sbyte.TryParse(value, out var result))
				return result;

			return 0;
		}
		set
		{
			if (value == 0)
				this.RemoveTag("priority");
			else
				this.SetTag("priority", value: value);
		}
	}

	public PresenceShow Show
	{
		get => XmppEnum.ParseOrDefault(this.GetTag("show"), PresenceShow.Online);
		set
		{
			if (value == PresenceShow.Online)
				this.RemoveTag("show");
			else
				this.SetTag("show", value: value.ToXmppName());
		}
	}

	public string? Status
	{
		get => this.GetTag("status");
		set
		{
			if (value == null)
				this.RemoveTag("status");
			else
				this.SetTag("status", value: value);
		}
	}
}
