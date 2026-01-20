using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Extensions.XEP0045;
using XmppSharp.Protocol.Extensions.XEP0172;

namespace XmppSharp.Protocol;

[Tag("precense", Namespaces.Client)]
[Tag("precense", Namespaces.Server)]
[Tag("precense", Namespaces.Component)]
public class Presence : Stanza
{
	public Presence() : base("presence", Namespaces.Client)
	{

	}

	public Presence(PresenceType type, PresenceShow show = default, sbyte? priority = default) : this()
	{
		Type = type;
		Show = show;
		Priority = priority;
	}

	public new PresenceType Type
	{
		get => XmppEnum.ParseOrDefault(base.Type, PresenceType.Available);
		set
		{
			if (value == PresenceType.Available)
				base.Type = null;
			else
			{
				if (!Enum.IsDefined(value))
					throw new ArgumentException(default, nameof(Type));

				base.Type = XmppEnum.ToXml(value);
			}
		}
	}

	public PresenceShow Show
	{
		get => XmppEnum.ParseOrDefault<PresenceShow>(GetTag("show"));
		set
		{
			if (Enum.IsDefined(value))
				SetTag("show", value: value.ToXml());
			else
				RemoveTag("show");
		}
	}

	public sbyte? Priority
	{
		get
		{
			if (sbyte.TryParse(GetTag("priority"), out var result))
				return result;

			return default;
		}
		set
		{
			if (!value.HasValue)
				RemoveTag("priority");
			else
				SetTag("priority", value: (sbyte)value);
		}
	}

	public string? Status
	{
		get => GetTag("status");
		set
		{
			if (value != null)
				SetTag("status", value: value);
			else
				RemoveTag("status");
		}
	}

	public MucUser? User
	{
		get => Element<MucUser>();
		set
		{
			Element<MucUser>()?.Remove();

			AddChild(value);
		}
	}

	public Nickname? Nickname
	{
		get => Element<Nickname>();
		set
		{
			Element<Nickname>()?.Remove();

			AddChild(value);
		}
	}
}
