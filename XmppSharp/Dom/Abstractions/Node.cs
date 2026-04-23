using XmppSharp.Xml;

namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Represents a node in the DOM tree. This can be an element, text, comment, etc.
/// </summary>
public abstract class Node : ICloneable
{
    internal Element? _parent;

    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Gets the parent element of this node. If the node is not attached to any element, this will return null.
    /// </summary>
    public Element? Parent => _parent;

    /// <summary>
    /// Creates a deep copy of this node.
    /// </summary>
    /// <returns>A deep copy of this node.</returns>
    public abstract Node Clone();

    /// <summary>
    /// Writes the XML representation of this node to the specified <see cref="DomWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="DomWriter"/> to write to.</param>
    public abstract void WriteTo(DomWriter writer);

    /// <summary>
    /// Removes this node from its parent element.
    /// </summary>
    public void Remove() => _parent?.RemoveChild(this);
}