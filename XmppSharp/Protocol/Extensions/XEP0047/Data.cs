using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0047;

[Tag("data", Namespaces.Ibb)]
public class Data : XmppElement
{
	public Data() : base("data", Namespaces.Ibb)
	{

	}

	public ushort? Seq
	{
		get => GetAttribute<ushort>("seq");
		set => SetAttribute("seq", value);
	}

	public string? SessionId
	{
		get => GetAttribute("sid");
		set => SetAttribute("sid", value);
	}

	public void SetBytes(byte[] buffer)
		=> InnerText = Convert.ToBase64String(buffer);
}