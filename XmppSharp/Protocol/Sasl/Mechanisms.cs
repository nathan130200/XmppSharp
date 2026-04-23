using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Sasl;

/// <summary>
/// The <see cref="Mechanisms"/> class represents the <![CDATA[<mechanisms>]]> element in the SASL feature.
/// </summary>
[Tag("mechanisms", Namespaces.Sasl)]
public class Mechanisms : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mechanisms"/> class.
    /// </summary>
    public Mechanisms() : base("mechanisms", Namespaces.Sasl)
    {

    }

    /// <summary>
    /// Gets or sets the supported mechanisms.
    /// <para>
    /// Each mechanism is represented by a <see cref="Mechanism"/> element.
    /// </para>
    /// </summary>
    public IEnumerable<Mechanism> SupportedMechanisms
    {
        get => Elements<Mechanism>();
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            Elements<Mechanism>()?.Remove();

            if (value?.Any() == true)
            {
                foreach (var item in value)
                    AddChild(item);
            }
        }
    }

    /// <summary>
    /// Checks if the specified mechanism is supported.
    /// </summary>
    /// <param name="name">The name of the mechanism to check.</param>
    /// <returns><c>true</c> if the mechanism is supported; otherwise, <c>false</c>.</returns>
    public bool SupportsMechanism(string name)
        => SupportedMechanisms.Any(x => x.InnerText == name);

    /// <summary>
    /// Adds a new mechanism to the list of supported mechanisms.
    /// </summary>
    /// <param name="name">The name of the mechanism to add.</param>
    /// <returns>The current <see cref="Mechanisms"/> instance.</returns>
    public Mechanisms AddMechanism(string name)
    {
        AddChild(new Mechanism(name));

        return this;
    }
}
