using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Client;

[XmppTag("bind", Namespace.Bind)]
public class Bind : Element
{
    public Bind() : base("bind", Namespace.Bind)
    {

    }

    public string Resource
    {
        get => GetTag("resource");
        set
        {
            if (value == null)
                RemoveTag("resource");
            else
                SetTag("resource", value);
        }
    }

    public Jid Jid
    {
        get
        {
            var jid = GetTag("jid");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set
        {
            if (!value.TryGetValue(out var result))
                RemoveTag("jid");
            else
                SetTag("jid", result.ToString());
        }
    }
}
