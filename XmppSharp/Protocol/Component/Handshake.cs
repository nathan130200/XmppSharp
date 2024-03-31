using System.Security.Cryptography;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Component;

/// <summary>
/// Represents a handshake element used in XMPP component communication for authentication purposes.
/// </summary>
[XmppTag("handshake", Namespaces.Accept)]
[XmppTag("handshake", Namespaces.Connect)]
public class Handshake : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Handshake"/> class.
    /// </summary>
    public Handshake() : base("handshake", Namespaces.Accept)
    {

    }

    /// <summary>
    /// Creates a handshake element using the SHA-1 hash of the concatenation of stream ID and password.
    /// </summary>
    /// <param name="streamId">The unique identifier of the XMPP stream.</param>
    /// <param name="password">The password associated with the component.</param>
    public Handshake(string streamId, string password) : this()
    {
        Content = GetHash(streamId, password);
    }

    /// <summary>
    /// Creates a handshake element using the provided hash.
    /// </summary>
    /// <param name="hash">The authentication token for the component.</param>
    public Handshake(string hash) : this()
        => Content = hash;

    /// <summary>
    /// Calculates the SHA-1 hash of the combined stream ID and password for authentication.
    /// </summary>
    /// <param name="sid">The stream ID.</param>
    /// <param name="pwd">The password.</param>
    /// <returns>The hexadecimal string representation of the SHA-1 hash.</returns>
    public static string GetHash(string sid, string pwd)
    {
        return SHA1.HashData(string.Concat(sid, pwd).GetBytes()).ToHex();
    }

    /// <summary>
    /// Verifies if the handshake value matches the SHA-1 hash of the provided stream ID and password.
    /// </summary>
    /// <param name="streamId">The stream ID used for comparison.</param>
    /// <param name="password">The password used for comparison.</param>
    /// <returns>True if the handshake value matches the hash, false otherwise.</returns>
    public bool HasAuthentication(string streamId, string password)
        => Content == GetHash(streamId, password);
}