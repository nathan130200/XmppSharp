using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("field", Namespaces.DataForms)]
public class Field : Element
{
	public Field() : base("field", Namespaces.DataForms)
	{

	}

	public string? Name
	{
		get => this.GetAttribute("var");
		set => this.SetAttribute("var", value);
	}

	public FieldType Type
	{
		get => XmppEnum.ParseOrDefault(this.GetAttribute("type"), FieldType.TextSingle);
		set => this.SetAttribute("type", value.ToXmppName());
	}

	public string? Label
	{
		get => this.GetAttribute("label");
		set => this.SetAttribute("label", value);
	}

	public string? Description
	{
		get => this.GetTag("desc");
		set => this.SetTag("desc", value: value);
	}

	public bool IsRequired
	{
		get => this.HasTag("required");
		set
		{
			if (!value)
				this.RemoveTag("required");
			else
				this.SetTag("required");
		}
	}

	public IEnumerable<string> Values
	{
		get
		{
			foreach (var element in this.Children("value", Namespaces.DataForms))
				yield return element.Value!;
		}
		set
		{
			Children("value", Namespaces.DataForms).Remove();

			foreach (var str in value)
				this.SetTag("value", Namespaces.DataForms, str);
		}
	}

	public IEnumerable<Option> Option
	{
		get => this.Children<Option>();
		set
		{
			this.Children<Option>().Remove();

			foreach (var item in value)
				this.AddChild(item);
		}
	}
}
