using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("query", Namespace.DiscoItems)]
public class DiscoItems : Element
{
	public DiscoItems() : base("query", Namespace.DiscoItems)
	{

	}

	public DiscoItems(string? node) : this()
	{
		this.Node = node;
	}

	public string? Node
	{
		get => this.GetAttribute("node");
		set => this.SetAttribute("node", value);
	}

	public IEnumerable<Item> Items
	{
		get => this.Children<Item>();
		set
		{
			this.Children<Item>().Remove();

			if (value != null)
			{
				foreach (var item in value)
					this.AddChild(item);
			}
		}
	}
}
