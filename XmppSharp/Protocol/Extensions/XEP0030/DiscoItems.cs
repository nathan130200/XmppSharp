using Jabber.Attributes;
using XmppSharp.Abstractions;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[StanzaExtension("query", Namespaces.DiscoItems)]
public sealed class DiscoItems : StanzaExtension
{
    List<Item>? _items;

    public DiscoItems()
    {

    }

    internal DiscoItems(Element element) : base(element)
    {
        Node = element.Attribute("node");

        foreach (var item in element.Elements("item", Namespaces.DiscoItems))
        {
            string? jid = item.Attribute("jid");

            if (string.IsNullOrWhiteSpace(jid)) continue;

            string? name = item.Attribute("name");

            string? node = item.Attribute("node");

            AddItem(new(jid, name, node));
        }
    }

    public string? Node { get; set; }

    public IEnumerable<Item> Items
    {
        get
        {
            lock (this)
            {
                if (_items == null) return [];

                return [.. _items];
            }
        }
    }

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

    public void RemoveItems()
    {
        lock (this)
        {
            if (_items == null) return;

            _items.Clear();
        }
    }

    public void AddItem(Item item)
    {
        lock (this)
        {
            _items ??= [];

            _items.Add(item);
        }
    }
}