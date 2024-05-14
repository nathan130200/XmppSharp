using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("x", Namespaces.DataForms)]
public class Form : Element
{
	public Form() : base("x", Namespaces.DataForms)
	{

	}

	public FormType Type
	{
		get => XmppEnum.ParseOrDefault(this.GetAttribute("type"), FormType.Form);
		set => this.SetAttribute("type", value.ToXmppName());
	}

	public string? Instructions
	{
		get => this.GetTag("instructions");
		set
		{
			if (value == null)
				this.RemoveTag("instructions");
			else
				this.SetTag("instructions", value: value);
		}
	}

	public string? Title
	{
		get => this.GetTag("title");
		set
		{
			if (value == null)
				this.RemoveTag("title");
			else
				this.SetTag("title", value: value);
		}
	}

	public Reported? Reported
	{
		get => this.Child<Reported>();
		set
		{
			this.Reported?.Remove();
			this.AddChild(value);
		}
	}

	public IEnumerable<Field> Fields
	{
		get => this.Children<Field>();
		set
		{
			this.Fields.Remove();

			foreach (var item in value)
				this.AddChild(item);
		}
	}
}
