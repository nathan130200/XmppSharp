using XmppSharp.Dom.Abstractions;
using XmppSharp.Xml;

namespace XmppSharp.Dom;

/// <summary>
/// Represents a CDATA section.
/// </summary>
/// <param name="value"></param>
public class Cdata(string value) : Node, IXmlContent
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
        return new Cdata(Value);
    }

    /// <inheritdoc/>
    public override void WriteTo(DomWriter writer)
    {
        writer.WriteCdata(Value);
    }
}