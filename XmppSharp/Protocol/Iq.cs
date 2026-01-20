using System.Diagnostics.CodeAnalysis;
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

	readonly record struct QueryInfo(string TagName, string? Namespace);

	static readonly List<QueryInfo> s_KnownQueries = new();

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

	public static void RegisterQuery(string tag, string? ns)
	{
		lock (s_KnownQueries)
		{
			var index = s_KnownQueries.FindIndex(x => x.TagName == tag && x.Namespace == ns);

			if (index == -1)
				s_KnownQueries.Add(new(tag, ns));
		}
	}

	public XmppElement? Query
	{
		get
		{
			QueryInfo[] entries;

			lock (s_KnownQueries)
				entries = s_KnownQueries.ToArray();

			foreach (var entry in entries)
			{
				if (FindQueryElement(entry.TagName, entry.Namespace, out var result))
					return result;
			}

			return default;
		}
		set
		{
			Query?.Remove();
			AddChild(value);
		}
	}

	bool FindQueryElement(string tagName, string? ns, [NotNullWhen(true)] out XmppElement? result)
	{
		result = Element(tagName, ns);
		return result != null;
	}
}