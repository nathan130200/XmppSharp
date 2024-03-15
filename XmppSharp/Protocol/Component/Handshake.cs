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
        Value = GetHash(streamId, password);
    }

    public Handshake(string token) : this()
        => Value = token;

    static string GetHash(string sid, string pwd)
    {
        return SHA1.HashData(string.Concat(sid, pwd).GetBytes()).GetHex();
    }

    public bool HasAuthentication(string streamId, string password)
        => Value == GetHash(streamId, password);
}