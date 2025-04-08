using System.Diagnostics.CodeAnalysis;
using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol;

[XmppTag("iq", Namespaces.Client)]
[XmppTag("iq", Namespaces.Accept)]
[XmppTag("iq", Namespaces.Connect)]
[XmppTag("iq", Namespaces.Server)]
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
        get => XmppEnum.FromXmlOrThrow<IqType>(base.Type);
        set => base.Type = XmppEnum.ToXmlOrThrow(value);
    }

    readonly record struct QueryInfo(string TagName, string? Namespace);

    static readonly List<QueryInfo> s_KnownQueries = new();

    static Iq()
    {
        RegisterQuery("bind", Namespaces.Bind);
        RegisterQuery("session", Namespaces.Session);
        RegisterQuery("ping", Namespaces.Ping);
        RegisterQuery("query", Namespaces.DiscoInfo);
        RegisterQuery("query", Namespaces.DiscoItems);
        RegisterQuery("query", Namespaces.CryOnline);
        RegisterQuery("query", Namespaces.IqAuth);
        RegisterQuery("query", Namespaces.IqGateway);
        RegisterQuery("query", Namespaces.IqLast);
        RegisterQuery("query", Namespaces.IqOob);
        RegisterQuery("query", Namespaces.IqPrivacy);
        RegisterQuery("query", Namespaces.IqPrivate);
        RegisterQuery("query", Namespaces.IqRegister);
        RegisterQuery("query", Namespaces.IqRoster);
        RegisterQuery("query", Namespaces.IqRpc);
        RegisterQuery("query", Namespaces.IqSearch);
        RegisterQuery("query", Namespaces.IqVersion);
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