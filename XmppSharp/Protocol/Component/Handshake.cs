using System.Security.Cryptography;
using System.Text;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Component;

/// <summary>
/// The handshake element is used in the XMPP protocol for component authentication.
/// </summary>
[Tag("handshake", Namespaces.Component)]
public class Handshake : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Handshake"/> class.
    /// </summary>
    public Handshake() : base("handshake", Namespaces.Component)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Handshake"/> class with the specified stream ID and secret.
    /// </summary>
    /// <param name="streamId">The stream ID used for generating the handshake hash.</param>
    /// <param name="secret">The secret key used for generating the handshake hash.</param>
    public Handshake(string streamId, string secret) : this()
    {
        InnerText = CreateHash(streamId, secret);
    }

    /// <summary>
    /// Creates a hash for the handshake element using the provided stream ID and secret.
    /// </summary>
    /// <param name="streamId">The stream ID used for generating the handshake hash.</param>
    /// <param name="secret">The secret key used for generating the handshake hash.</param>
    /// <returns>The generated hash as a hexadecimal string.</returns>
    public static string CreateHash(string streamId, string secret)
    {
        var hash = SHA1.HashData(Encoding.UTF8.GetBytes(string.Concat(streamId, secret)));

        return Convert.ToHexStringLower(hash);
    }
}