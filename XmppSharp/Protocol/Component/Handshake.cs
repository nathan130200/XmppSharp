using System.Security.Cryptography;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Component;

[XmppTag("handshake", Namespaces.Accept)]
[XmppTag("handshake", Namespaces.Connect)]
public class Handshake : Element
{
    public Handshake() : base("handshake", Namespaces.Accept)
    {

    }

    public Handshake(string streamId, string password) : this()
    {
        Value = GetAuthenticationHash(streamId, password);
    }

    public bool HasAuthentication(string streamId, string password)
    {
        if (string.IsNullOrWhiteSpace(Value))
            throw new InvalidOperationException("Missing authentication token.");

        return GetAuthenticationHash(streamId, password).Equals(Value, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetAuthenticationHash(string streamId, string password)
    {
        var hash = SHA1.HashData(string.Concat(streamId, password).GetBytes());
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}