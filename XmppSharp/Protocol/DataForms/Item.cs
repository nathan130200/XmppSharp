using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.DataForms;

/// <summary>
/// Represents an "item" element used within Data Forms in XMPP.
/// <para>This element serves as a container for a single field within a data form.</para>
/// <para>It can hold various field types, such as text, lists, or boolean values.</para>
/// </summary>
[XmppTag("item", Namespaces.DataForms)]
public class Item : Element
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Item"/> class.
    /// </summary>
    public Item() : base("item", Namespaces.DataForms)
    {

    }

    /// <summary>
    /// Gets or sets the field contained within the item.
    /// The field object defines the specific data type and value associated with this item.
    /// </summary>
    public Field? Field
    {
        get => Child<Field>();
        set => ReplaceChild(value);
    }
}
