using XmppSharp.Attributes;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[Tag("presence", Namespaces.Client)]
[Tag("presence", Namespaces.Server)]
[Tag("presence", Namespaces.Component)]
public sealed class Presence() : Stanza("presence")
{
	public PresenceType Type
	{
		get => XmppEnum<PresenceType>.Parse(GetAttribute("type")!);
		set => SetAttribute("type", XmppEnum<PresenceType>.GetName(value));
	}

	public sbyte? Priority
	{
		get
		{
			if (sbyte.TryParse(GetTag("priority", Namespace), out var result))
				return result;

			return null;
		}
		set
		{
			if (value is null)
				RemoveTag("priority", Namespace);
			else
				SetTag("priority", Namespace, value);
		}
	}
}