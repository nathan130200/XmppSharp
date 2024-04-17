namespace XmppSharp.Protocol.Base;

public abstract class DirectionalElement : Element
{
	protected DirectionalElement(string qualifiedName) : base(qualifiedName)
	{
	}

	protected DirectionalElement(string qualifiedName, string namespaceURI) : base(qualifiedName, namespaceURI)
	{
	}

	public Jid From
	{
		get
		{
			var jid = this.GetAttribute("from");

			if (Jid.TryParse(jid, out var result))
				return result;

			return null;
		}
		set => this.SetAttribute("from", value?.ToString());
	}

	public Jid To
	{
		get
		{
			var jid = this.GetAttribute("to");

			if (Jid.TryParse(jid, out var result))
				return result;

			return null;
		}
		set => this.SetAttribute("to", value?.ToString());
	}

	public void SwitchDirection()
		=> (this.From, this.To) = (this.To, this.From);
}
