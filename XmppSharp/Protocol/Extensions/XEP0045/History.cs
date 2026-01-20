using System.Xml;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[Tag("history", Namespaces.Muc)]
public class History : XmppElement
{
	public History() : base("history", Namespaces.Muc)
	{

	}

	public int? MaxChars
	{
		get => GetAttribute<int>("maxchars");
		set
		{
			if (!value.HasValue)
				RemoveAttribute("maxchars");
			else
				SetAttribute("maxchars", (int)value);
		}
	}

	public int? MaxStanzas
	{
		get => GetAttribute<int>("maxstanzas");
		set
		{
			if (!value.HasValue)
				RemoveAttribute("maxstanzas");
			else
				SetAttribute("maxstanzas", (int)value);
		}
	}

	public int? Seconds
	{
		get => GetAttribute<int>("seconds");
		set
		{
			if (!value.HasValue)
				RemoveAttribute("seconds");
			else
				SetAttribute("seconds", (int)value);
		}
	}

	public DateTimeOffset? Since
	{
		get
		{
			if (HasAttribute("since"))
				return XmlConvert.ToDateTimeOffset(GetAttribute("since")!);

			return default;
		}
		set
		{
			if (!value.HasValue)
				RemoveAttribute("since");
			else
				SetAttribute("since", XmlConvert.ToString(value.Value));
		}
	}
}