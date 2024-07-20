namespace XmppSharp.Protocol.Base;

public abstract class Stanza : DirectionalElement
{
	protected Stanza(string qualifiedName) : base(qualifiedName)
	{
	}

	protected Stanza(string qualifiedName, string namespaceURI) : base(qualifiedName, namespaceURI)
	{
	}

	public string? Id
	{
		get => this.GetAttribute("id");
		set => this.SetAttribute("id", value);
	}

	public string? Type
	{
		get => this.GetAttribute("type");
		set => this.SetAttribute("type", value);
	}

	public string? Language
	{
		get => this.GetAttribute("xml:lang");
		set => this.SetAttribute("xml:lang", value);
	}

	public void GenerateId()
		=> this.Id = Guid.NewGuid().ToString("d");

	public StanzaError? Error
	{
		get => this.Child<StanzaError>();
		set
		{
			this.RemoveTag("error", Namespaces.Stanzas);

			if (value != null)
				this.AddChild(value);
		}
	}
}