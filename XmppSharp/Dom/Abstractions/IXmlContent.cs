namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Defines the interface for XML content nodes, which represent text content within an XML element (eg: Text and CDATA nodes).
/// </summary>
public interface IXmlContent
{
    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    string Value { get; set; }
}