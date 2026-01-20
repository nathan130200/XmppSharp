using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0172;

[Tag("nick", Namespaces.Nick)]
public class Nickname : XmppElement
{
	public Nickname() : base("nick", Namespaces.Nick)
	{

	}

	public Nickname(string value) : this()
	{
		InnerText = value;
	}
}
