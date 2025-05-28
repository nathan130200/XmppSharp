using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("mechanism", Namespaces.Sasl)]
public class Mechanism : XmppElement, ISaslMechanism
{
    public Mechanism() : base("mechanism", Namespaces.Sasl)
    {

    }

    public Mechanism(string mechanismName) : this()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mechanismName);
        Value = mechanismName;
    }

    string? ISaslMechanism.MechanismName => Value;
}

public interface ISaslMechanism
{
    string? MechanismName { get; }
}
