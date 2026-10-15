using System.Diagnostics;

namespace XmppSharp.Dom.Abstractions;

/// <summary>
/// Represents a base class of Container node.
/// </summary>
public abstract class Container : Node
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal readonly List<Node> _children = [];

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static readonly IEnumerable<Node> s_EmptyNodeList = [];

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static readonly IEnumerable<Element> s_EmptyElementList = [];

	/// <summary>
	/// Gets the first node.
	/// </summary>
	public Node? FirstNode
	{
		get
		{
			lock (_children)
				return _children.Count > 0 ? _children[0] : null;
		}
	}

	/// <summary>
	/// Gets the last node.
	/// </summary>
	public Node? LastNode
	{
		get
		{
			lock (_children)
				return _children.Count > 0 ? _children[^1] : null;
		}
	}

	/// <summary>
	/// Gets a collection of child nodes.
	/// </summary>
	public IEnumerable<Node> Nodes()
	{
		List<Node>? result = null;

		lock (_children)
		{
			if (_children.Count > 0)
			{
				result = [];

				foreach (var node in _children)
					result.Add(node);
			}
		}

		return result ?? s_EmptyNodeList;
	}

	/// <summary>
	/// Gets a collection of <see cref="Dom.Element" /> nodes.
	/// </summary>
	public IEnumerable<Element> Elements()
	{
		List<Element>? result = null;

		lock (_children)
		{
			foreach (var node in _children)
			{
				if (node is Element e)
				{
					result ??= [];

					result.Add(e);
				}
			}
		}

		return result ?? s_EmptyElementList;
	}

	/// <summary>
	/// Gets the first matching element occurrence.
	/// </summary>
	/// <param name="name">Element qualified name.</param>
	/// <param name="ns">Namespace URI.</param>
	/// <remarks>If <paramref name="ns"/> parameter is <see langword="null" /> it will inherit the namespace of the current element.</remarks>
	public Element? Element(string name, string? ns = default)
	{
		ns ??= (this as Element)?.Namespace;

		Element? result = null;

		foreach (var element in Elements())
		{
			if (element.Name == name && element.Namespace == ns)
			{
				result = element;
				break;
			}
		}

		return result;
	}

	/// <summary>
	/// Gets an collection of matching elements.
	/// </summary>
	/// <param name="name">Element qualified name.</param>
	/// <param name="ns">Namespace URI.</param>
	/// <remarks>If <paramref name="ns"/> parameter is <see langword="null" /> it will inherit the namespace of the current element.</remarks>
	public IEnumerable<Element> Elements(string name, string? ns = default)
	{
		ns ??= (this as Element)?.Namespace;

		foreach (var element in Elements())
		{
			if (element.Name == name && element.Namespace == ns)
				yield return element;
		}
	}

	/// <summary>
	/// Remove all child nodes.
	/// </summary>
	public void RemoveNodes()
	{
		lock (_children)
		{
			foreach (var node in _children)
				node._parent = null;

			_children.Clear();

			_children.Capacity = 0;
		}
	}

	/// <summary>
	/// Gets a collection of descendant nodes of this element, in document order.
	/// </summary>
	public IEnumerable<Node> DescendantNodes()
	{
		List<Node>? result = null;
		BuildDescendantNodesList(this, ref result);
		return result ?? s_EmptyNodeList;
	}

	/// <summary>
	/// Gets a collection of current node, its descendant nodes, in document order.
	/// </summary>
	public IEnumerable<Node> DescendantNodesAndSelf()
	{
		yield return this;

		foreach (var node in DescendantNodes())
			yield return node;
	}

	static void BuildDescendantNodesList(Container source, ref List<Node>? result)
	{
		foreach (var node in source.Nodes())
		{
			result ??= [];

			result.Add(node);

			if (node is Container other)
				BuildDescendantNodesList(other, ref result);
		}
	}
}
