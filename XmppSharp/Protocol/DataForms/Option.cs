using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("option", Namespace.DataForms)]
public class Option : Element
{
	public Option() : base("option", Namespace.DataForms)
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