using System.Diagnostics.CodeAnalysis;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

/// <summary>
/// This is a implementation of disco info extension.
/// </summary>
[StanzaExtension("query", Namespaces.DiscoItems)]
public sealed class DiscoItems : BaseStanzaExtension
{
	IEnumerable<Item>? _items = [];

	/// <summary>
	/// Initialize a new instance of <see cref="DiscoItems"/> class.
	/// </summary>
	public DiscoItems()
	{

	}

	internal DiscoItems(Element element) : base(element)
	{
		Node = element.Attribute("node");

		List<Item>? items = null;

		foreach (var item in element.Elements("item", Namespaces.DiscoItems))
		{
			string? jid = item.Attribute("jid");

			if (string.IsNullOrWhiteSpace(jid)) continue;

			string? name = item.Attribute("name");

			string? node = item.Attribute("node");

			items ??= [];

			items.Add(new Item(jid, name, node));
		}

		_items = items;
	}

	/// <summary>
	/// Gets or sets the node identifier.
	/// </summary>
	public string? Node
	{
		get;
		set;
	}

	/// <summary>
	/// Gets list of items.
	/// </summary>
	[NotNull]
	public IEnumerable<Item>? Items
	{
		get
		{
			lock (this)
			{
				if (_items is null) return [];

				return _items;
			}
		}
		set
		{
			lock (this)
			{
				_items = value;
			}
		}
	}

	/// <inheritdoc/>
	public override Element Serialize()
	{
		var result = new Element("query", Namespaces.DiscoItems);

		result.SetAttribute("node", Node);

		lock (this)
		{
			if (_items != null)
			{
				foreach (var item in _items)
				{
					result.C("item", x =>
					{
						x.SetAttribute("jid", item.Jid);
						x.SetAttribute("name", item.Name);
						x.SetAttribute("node", item.Node);
					});
				}
			}
		}

		return result;
	}
}