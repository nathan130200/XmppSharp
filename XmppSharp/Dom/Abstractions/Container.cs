namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Represents a node that can contain other nodes. This is the base class for elements and documents.
/// </summary>
public abstract class Container : Node
{
    internal readonly List<Node> _children = [];

    /// <summary>
    /// Gets the child nodes of this container.
    /// </summary>
    /// <returns>
    /// A snapshot of the current state of the container's children.
    /// </returns>
    public IEnumerable<Node> Nodes()
    {
        lock (_children)
            return [.. _children];
    }

    /// <summary>
    /// Gets all descendant nodes of this container.
    /// </summary>
    /// <returns>
    /// A snapshot of the current state of the container's descendant nodes.
    /// </returns>
    public IEnumerable<Node> DescendantNodes()
    {
        List<Node> result = [];
        BuildDescendantNodesList(this, result);
        return result;
    }

    internal static void BuildDescendantNodesList(Container context, List<Node> result)
    {
        foreach (var node in context.Nodes())
        {
            result.Add(node);

            if (node is Container container)
                BuildDescendantNodesList(container, result);
        }
    }

    /// <summary>
    /// Gets this container and all descendant nodes of this container.
    /// </summary>
    /// <returns>
    /// A snapshot of the current state of the container and its descendant nodes.
    /// </returns>
    public IEnumerable<Node> DescendantNodesAndSelf()
    {
        yield return this;

        foreach (var node in DescendantNodes())
            yield return node;
    }

    /// <summary>
    /// Gets the child elements of this container.
    /// </summary>
    /// <returns>
    /// A snapshot of the current state of the container's child elements.
    /// </returns>
    public IEnumerable<Element> Elements()
        => Nodes().OfType<Element>();

    /// <summary>
    /// Gets the first child node of this container, or null if there are no child nodes.
    /// </summary>
    public Node? FirstNode
        => Nodes().FirstOrDefault();

    /// <summary>
    /// Gets the last child node of this container, or null if there are no child nodes.
    /// </summary>
    public Node? LastNode
        => Nodes().LastOrDefault();

    /// <summary>
    /// Removes all child nodes from this container.
    /// </summary>
    public void RemoveNodes()
    {
        lock (_children)
        {
            foreach (var node in _children)
                node._parent = null;

            _children.Clear();
        }
    }

    /// <summary>
    /// Gets the first child element with the specified name and namespace URI, or null if no such element exists.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="namespaceUri"></param>
    /// <returns></returns>
    public Element? Element(string name, string? namespaceUri = default)
        => Elements(name, namespaceUri).FirstOrDefault();

    /// <summary>
    /// Gets the first child element of the specified type, or null if no such element exists.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the element to find. This must be a subclass of <see cref="Dom.Element"/>.
    /// </typeparam>
    /// <returns>
    /// The first child element of the specified type, or null if no such element exists.
    /// </returns>
    public T? Element<T>() where T : Element
        => Elements().OfType<T>().FirstOrDefault();

    /// <summary>
    /// Gets the child elements with the specified name and namespace URI.
    /// </summary>
    /// <param name="name">The name of the elements to find.</param>
    /// <param name="namespaceUri">The namespace URI of the elements to find.</param>
    /// <returns>
    /// A collection of child elements with the specified name and namespace URI.
    /// </returns>
    public IEnumerable<Element> Elements(string name, string? namespaceUri = default)
        => Elements().Where(x => x.TagName == name && namespaceUri == null || x.NamespaceUri == namespaceUri);

    /// <summary>
    /// Gets the child elements of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the elements to find. This must be a subclass of <see cref="Dom.Element"/>.</typeparam>
    /// <returns>
    /// A collection of child elements of the specified type.
    /// </returns>
    public IEnumerable<T> Elements<T>() where T : Element
        => Elements().OfType<T>();
}
