using XmppSharp.Protocol;

namespace XmppSharp.Entities.Options;

/// <summary>
/// Represents the XMPP client connection settings.
/// </summary>
public class XmppClientConnectionOptions : XmppConnectionOptions
{
    /// <summary>
    /// Host qualified domain name of the server.
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Username to log in. Default: Machine's user name.
    /// </summary>
    public string Username { get; set; } = Environment.UserName;

    /// <summary>
    /// The resource is a string that uniquely identifies the connection on the server.
    /// <para>
    /// It also allows you to connect with the same username and password on the server (based on the server's internal policy).
    /// </para>
    /// <para>
    /// Omitting the resource may allow the server to generate one or the server may refuse with <b>bad request</b> as error.
    /// </para>
    /// </summary>
    public string? Resource { get; set; }

    /// <summary>
    /// Authentication mechanism. Default: <c>PLAIN</c>
    /// <para>
    /// For an authentication mechanism to be instantiated, it must be declared and registered correctly via <see cref="Sasl.SaslFactory" />
    /// </para>
    /// </summary>
    public string? AuthenticationMechanism { get; set; } = "PLAIN";

    /// <summary>
    /// Determines whether the connection will respond to ping requests automatically.
    /// </summary>
    public bool AutoPing { get; set; }

    /// <summary>
    /// Determines the initial presence that will be sent after connecting (visibility, status, etc).
    /// </summary>
    public Presence InitialPresence { get; set; }
}
