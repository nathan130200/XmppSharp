using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[Tag("status", Namespaces.MucUser)]
public class Status : XmppElement
{
	public Status() : base("status", Namespaces.MucUser)
	{

	}

	public Status(int code) : this()
	{
		Code = code;
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
}
