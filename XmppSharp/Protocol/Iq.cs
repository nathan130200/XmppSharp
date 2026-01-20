using XmppSharp.Abstractions;
using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[Tag("iq", Namespaces.Client)]
[Tag("iq", Namespaces.Component)]
[Tag("iq", Namespaces.Server)]
public class Iq : Stanza
{
	public Iq() : base("iq")
	{

	}

	public Iq(IqType type) : this()
	{
		Type = type;
	}

	public new IqType Type
	{
		get => XmppEnum.Parse<IqType>(base.Type);
		set => base.Type = XmppEnum.ToXml(value);
	}

	static readonly HashSet<XmlTagInfo> s_QueryTypes = new();

	static Iq()
	{
		RegisterQuery("bind", Namespaces.Bind);
		RegisterQuery("session", Namespaces.Session);
		RegisterQuery("ping", Namespaces.Ping);

		string[] namespaces = [
			Namespaces.DiscoInfo,
			Namespaces.DiscoItems,
			Namespaces.CryOnline,
			Namespaces.IqAuth,
			Namespaces.IqGateway,
			Namespaces.IqLast,
			Namespaces.IqOob,
			Namespaces.IqPrivacy,
			Namespaces.IqPrivate,
			Namespaces.IqRegister,
			Namespaces.IqRoster,
			Namespaces.IqRpc,
			Namespaces.IqSearch,
			Namespaces.IqVersion,
		];

		foreach (var ns in namespaces)
			RegisterQuery("query", ns);

		RegisterQuery("vCard", Namespaces.vCard);
	}

	public static void RegisterQuery(string tagName, string namespaceURI)
	{
		lock (s_QueryTypes)
			s_QueryTypes.Add(new(tagName, namespaceURI));
	}

	XmppElement? FindQueryElement()
	{
		XmppElement? result = null;

		XmlTagInfo[] types;

		lock (s_QueryTypes)
			types = [.. s_QueryTypes];

		foreach (var type in types)
		{
			if (Element(type.Name, type.Namespace) is XmppElement temp)
			{
				result = temp;
				break;
			}
		}

		return result;
	}

	public XmppElement? Query
	{
		get => FindQueryElement();
		set
		{
			RemoveNodes();

			AddChild(value);
		}
	}
}