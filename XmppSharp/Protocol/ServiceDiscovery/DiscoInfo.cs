using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("query", Namespace.DiscoInfo)]
public class DiscoInfo : Element
{
	public DiscoInfo() : base("query", Namespace.DiscoInfo)
	{
	}

	public DiscoInfo(string? node) : this()
	{
		this.Node = node;
	}

	public DiscoInfo(string? node, IEnumerable<Identity>? identities, IEnumerable<Feature>? features) : this(node)
	{
		this.Identities = identities;
		this.Features = features;
	}

	public string? Node
	{
		get => this.GetAttribute("node");
		set => this.SetAttribute("node", value);
	}

	public IEnumerable<Identity> Identities
	{
		get => this.Children<Identity>();
		set
		{
			this.Children<Identity>().Remove();

			if (value != null)
			{
				foreach (var item in value)
					this.AddChild(item);
			}
		}
	}

	public IEnumerable<Feature> Features
	{
		get => this.Children<Feature>();
		set
		{
			this.Children<Feature>().Remove();

			if (value != null)
			{
				foreach (var feature in value)
					this.AddChild(feature);
			}
		}
	}
}
