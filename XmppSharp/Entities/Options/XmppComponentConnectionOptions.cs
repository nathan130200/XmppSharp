namespace XmppSharp.Entities.Options;

/// <summary>
/// Represents the XMPP component connection settings.
/// </summary>
public class XmppComponentConnectionOptions : XmppConnectionOptions
{
    /// <summary>
    /// Host qualified domain name of the component.
    /// </summary>
    public string Domain { get; set; }

    protected internal override string DefaultNamespace => Namespaces.Accept;
}