using System.Security.Cryptography;
using System.Xml.Linq;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Component;

[XmppTag("handshake", "jabber:component:accept")]
[XmppTag("handshake", "jabber:component:connect")]
public class Handshake : XElement
{
    public Handshake() : base("handshake", Namespace.Accept)
    {

    }

    public Handshake(string streamId, string password) : this()
    {
        Value = GetAuthenticationHash(streamId, password);
    }

    /// <summary>
    /// Calculates the SHA-1 hash of the combined stream ID and password for authentication.
    /// </summary>
    /// <param name="streamId">The stream ID.</param>
    /// <param name="pwd">The password.</param>
    /// <returns>The hexadecimal string representation of the component handshake value.</returns>
    public static string GetAuthenticationHash(string streamId, string pwd)
    {
        return SHA1.HashData(string.Concat(streamId, pwd).GetBytes()).ToHex();
    }

    /// <summary>
    /// Verifies if the handshake value matches the hash of the provided stream ID and password.
    /// </summary>
    /// <param name="streamId">The stream ID used for comparison.</param>
    /// <param name="password">The password used for comparison.</param>
    /// <returns>True if the handshake value matches the hash, false otherwise.</returns>
    public bool HasAuthentication(string streamId, string password)
        => Value == GetAuthenticationHash(streamId, password);
}