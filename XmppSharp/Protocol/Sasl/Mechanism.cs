using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : XmppElement, ISaslMechanismEntry
{
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    public Mechanism(string mechanismName) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mechanismName);
        Value = mechanismName;
    }

    string? ISaslMechanismEntry.MechanismName => Value;
}

public interface ISaslMechanismEntry
{
    string? MechanismName { get; }
}
