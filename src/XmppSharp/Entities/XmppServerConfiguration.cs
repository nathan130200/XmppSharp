using System.Security.Cryptography;

namespace Jabber.Entities;

public sealed class XmppServerConfiguration
{
    /// <summary>
    /// Defines what the encryption policy is on the server.
    /// </summary>
    public TlsPolicy TlsPolicy { internal get; set; } = TlsPolicy.Optional;

    /// <summary>
    /// Defines which IP and port the server will listen to.
    /// </summary>
    public (string Host, ushort Port) EndPoint { internal get; set; } = ("0.0.0.0", 5222);

    /// <summary>
    /// Defines the number of sockets that were in the queue waiting until they were accepted by the server.
    /// </summary>
    public int ListenBacklog { internal get; set; } = 10;

    /// <summary>
    /// Defines the size of bytes that the parser's character buffer will store to read the XML.
    /// </summary>
    public ushort ParserBufferSize { internal get; set; } = 1024;

    /// <summary>
    /// Qualified host name for the server.
    /// </summary>
    public string Hostname { internal get; set; } = "localhost";

    /// <summary>
    /// List of all internal accounts on the server. These accounts do not need to connect to a database to obtain credentials.
    /// </summary>
    public IEnumerable<(string User, string Hash)> SystemAccounts { internal get; set; } = [
        ("test", "9ADB6E0E46BC1DF4EEA726BC67E19D64"),
    ];

    /// <summary>
    /// The maximum time in which the server must wait to terminate active connections gracefully, after this time all connections are forced to terminate.
    /// </summary>
    public TimeSpan GracefullyDisconnectTimeout { internal get; set; } = TimeSpan.FromSeconds(15d);

    internal bool IsSystemAccount(string user, string password)
    {
        var buf = string.Concat(user, ':', password).GetBytes();
        var hash = Convert.ToHexString(MD5.HashData(buf));
        return SystemAccounts.Any(x => x.User == user && string.Equals(x.Hash, hash, StringComparison.OrdinalIgnoreCase));
    }
}
