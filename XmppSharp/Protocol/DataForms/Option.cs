using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("option", Namespaces.DataForms)]
public class Option : Element
{
	public Option() : base("option", Namespaces.DataForms)
	{

	}

	public string? Label
	{
		get => this.GetAttribute("label");
		set => this.SetAttribute("label", value);
	}

	public new string Value
	{
		get => this.GetTag("value");
		set => this.SetTag("value", value: value);
	}
}