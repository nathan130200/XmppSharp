using XmppSharp.Attributes;

namespace XmppSharp.Protocol.DataForms;

[XmppTag("reported", Namespace.DataForms)]
public class Reported : Element
{
	public Reported() : base("reported", Namespace.DataForms)
	{

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