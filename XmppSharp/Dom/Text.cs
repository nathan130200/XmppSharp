using XmppSharp.Dom.Abstractions;
using XmppSharp.Xml;

namespace XmppSharp.Dom;

/// <summary>
/// Represents a text content.
/// </summary>
/// <param name="value">
/// The text content of the node.
/// </param>
public class Text(string value) : Node, IXmlContent
{
    /// <inheritdoc/>
    public string Value
    {
        get;
        set;
    } = value;

    /// <inheritdoc/>
    public override Node Clone()
    {
        return new Text(Value);
    }

    /// <inheritdoc/>
    public override void WriteTo(DomWriter writer)
    {
        writer.WriteText(Value);
    }
}