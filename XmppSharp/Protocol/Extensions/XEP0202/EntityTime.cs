using System.Xml;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0202;

[Tag("time", Namespaces.EntityTime)]
public class EntityTime : XmppElement
{
	public EntityTime() : base("time", Namespaces.EntityTime)
	{

	}

	public DateTimeOffset? UtcTime
	{
		get
		{
			if (GetTag("utc") is string s)
				return XmlConvert.ToDateTimeOffset(s);

			return default;
		}
		set
		{
			RemoveTag("utc");

			if (value != null)
				SetTag("utc", value: XmlConvert.ToString(value.Value));
		}
	}

	public string? TimeZone
	{
		get => GetTag("tzo");
		set
		{
			RemoveTag("tzo");

			if (value != null)
				SetTag("tzo", value: value);
		}
	}
}
