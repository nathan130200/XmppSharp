using System.Security.Cryptography;
using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Component;

[XmppTag("handshake", Namespace.Accept)]
[XmppTag("handshake", Namespace.Connect)]
public class Handshake : Element
{
    public Handshake() : base("handshake", Namespace.Accept)
    {

    }

    public Handshake(string streamId, string password) : this()
    {
        Value = SHA1.HashData(string.Concat(streamId, password).GetBytes()).GetHex();
    }

    public Handshake(string token) : this()
        => Value = token;
}