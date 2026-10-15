using System.Runtime.CompilerServices;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

/// <summary>
/// Represents a collection of XML extensions.
/// </summary>
public static class DomExtensions
{
	extension(Element context)
	{
		/// <summary>
		/// Add child nodes to current node.
		/// </summary>
		/// <param name="nodes">Nodes to add.</param>
		public Element C(params Node[] nodes)
		{
			foreach (var node in nodes)
				context.AddChild(node);

			return context;
		}

		/// <summary>
		/// Add new child element to current element.
		/// </summary>
		/// <param name="name">XML qualified name.</param>
		/// <param name="ns">XML namespace uri.</param>
		[OverloadResolutionPriority(0)]
		public Element C(string name, string? ns = default)
		{
			var el = new Element(name, ns ?? context.Namespace);

			context.AddChild(el);

			return el;
		}

		/// <summary>
		/// Add new child element to current element and modify it.
		/// </summary>
		/// <param name="name">XML qualified name.</param>
		/// <param name="factory">Callback function to modify new created element.</param>
		[OverloadResolutionPriority(1)]
		public Element C(string name, Action<Element> factory)
			=> context.C(name, context.Namespace, factory);

		/// <summary>
		/// Add new child element to current element and modify it.
		/// </summary>
		/// <param name="name">XML qualified name.</param>
		/// <param name="ns">XML namespace uri.</param>
		/// <param name="factory">Callback function to modify new created element.</param>
		[OverloadResolutionPriority(2)]
		public Element C(string name, string? ns, Action<Element> factory)
		{
			ArgumentNullException.ThrowIfNull(factory);

			var el = new Element(name, ns ?? context.Namespace);

			context.AddChild(el);

			factory(el);

			return context;
		}

		/// <summary>
		/// Gets the parent element or self.
		/// </summary>
		public Element Up()
		{
			return context._parent ?? context;
		}

		/// <summary>
		/// Gets the root element by traversing each parent from current element.
		/// </summary>
		/// <returns></returns>
		public Element Root()
		{
			var scope = context;

			while (scope._parent != null)
				scope = scope._parent;

			return scope;
		}
	}
}
