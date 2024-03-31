using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Represents a "reported" element used within Data Forms in XMPP.
/// <para>This element encapsulates a collection of fields that provide additional information of a data form fields.</para>
/// </summary>
[XmppTag("reported", Namespaces.DataForms)]
public class Reported : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Reported"/> class.
    /// </summary>
    public Reported() : base("reported", Namespaces.DataForms)
    {

    }

    /// <summary>
    /// Gets or sets the collection of fields.
    /// </summary>
    public IEnumerable<Field> Fields
    {
        get => Children<Field>();
        set
        {
            Children<Field>().Remove();

            foreach (var element in value)
                AddChild(element);
        }
    }
}