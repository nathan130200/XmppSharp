using System.Collections.Concurrent;
using XmppSharp.Attributes;
using XmppSharp.Collections;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Protocol.Core;

[XmppTag("iq", Namespaces.Client)]
[XmppTag("iq", Namespaces.Accept)]
[XmppTag("iq", Namespaces.Connect)]
[XmppTag("iq", Namespaces.Server)]
public class Iq : Stanza
{
    public Iq(Iq other) : base(other)
    {

    }

    public Iq() : base("iq", Namespaces.Client)
    {

    }

    public Iq(IqType type) : this()
    {
        Type = type;
    }

    public new IqType Type
    {
        get
        {
            return base.Type switch
            {
                "get" => IqType.Get,
                "set" => IqType.Set,
                "result" => IqType.Result,
                "error" => IqType.Error,
                _ => throw new ArgumentException(default, nameof(Type))
            };
        }
        set
        {
            if (!Enum.IsDefined(value))
                throw new ArgumentException(default, nameof(Type));

            base.Type = XmppEnum.ToXml(value);
        }
    }

    // let store lib (aka "default") query tags as 0, and custom defined queries as 1
    static ConcurrentDictionary<XmppTag, byte> s_KnownQueries = new()
    {
        [new("bind", Namespaces.Bind)] = 0,
        [new("session", Namespaces.Session)] = 0,
        [new("query", Namespaces.CryOnline)] = 0,
        [new("ping", Namespaces.Ping)] = 0,
        [new("vCard", Namespaces.vCard)] = 0,
    };

    static Iq()
    {
        foreach (var field in typeof(Namespaces).GetFields())
        {
            if (!field.Name.StartsWith("Iq", StringComparison.Ordinal))
                continue;

            if (field.GetValue(null) is string str)
                s_KnownQueries[new("query", str)] = 0;
        }
    }

    /// <summary>
    /// Adds a new XML definition to map to the specified query.
    /// <para>
    /// The mapping will get the supported elements and make them available in the <see cref="Query" /> property.
    /// </para>
    /// </summary>
    /// <param name="tagName">Qualified tag name of the element.</param>
    /// <param name="namespaceURI">Namespace URI of the element.</param>
    public static void AddKnownQuery(string tagName, string namespaceURI)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);
        ThrowHelper.ThrowIfNullOrWhiteSpace(namespaceURI);

        var key = new XmppTag(tagName, namespaceURI);
        s_KnownQueries.TryAdd(key, 1);
    }

    public Element? Query
    {
        get
        {
            Element? result = default;

            foreach (var tag in s_KnownQueries.Keys)
            {
                var temp = Child(tag.Name, tag.Namespace);

                if (temp != null)
                {
                    result = temp;
                    break;
                }
            }

            return result;
        }
        set
        {
            // avoid remove other elements (eg: error, etc)

            foreach (var tag in s_KnownQueries.Keys)
                Child(tag.Name, tag.Namespace)?.Remove();

            AddChild(value);
        }
    }
}