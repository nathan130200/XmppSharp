using System.Xml;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0203;

[Tag("delay", Namespaces.Delay)]
public class Delay : XmppElement
{
	public Delay() : base("delay", Namespaces.Delay)
	{

	}

	public Delay(Jid? from, DateTimeOffset? stamp) : this()
	{
		From = from;
		Stamp = stamp;
	}

	public Jid? From
	{
		get => GetAttribute("from");
		set => SetAttribute("from", value);
	}

	public DateTimeOffset? Stamp
	{
		get
		{
			if (GetAttribute("stamp") is string s)
				return XmlConvert.ToDateTimeOffset(s);

			return default;
		}
		set
		{
			if (!value.HasValue)
				RemoveAttribute("stamp");
			else
				SetAttribute("stamp", Convert.ToString(value.Value));
		}
	}
}
