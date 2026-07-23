using XmppSharp.Dom;

namespace XmppSharp.Protocol.Base;

public abstract class DirectionalElement : Element
{
	protected DirectionalElement(string name, string? ns, object? value = null) : base(name, ns, value)
	{

	}

	public Jid? From
	{
		get => GetAttribute("from");
		set => SetAttribute("from", value);
	}

	public Jid? To
	{
		get => GetAttribute("to");
		set => SetAttribute("to", value);
	}

	public void SwitchDirection()
	{
		string? temp1 = GetAttribute("from"),
			temp2 = GetAttribute("to");

		SetAttribute("from", temp2);

		SetAttribute("to", temp1);
	}
}