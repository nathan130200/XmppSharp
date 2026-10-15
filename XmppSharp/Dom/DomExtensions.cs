using System.Runtime.CompilerServices;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

public static class DomExtensions
{
	extension(Element context)
	{
		public Element C(params Node[] nodes)
		{
			foreach (var node in nodes)
				context.AddChild(node);

			return context;
		}

		[OverloadResolutionPriority(0)]
		public Element C(string name, string? ns = default)
		{
			var el = new Element(name, ns ?? context.Namespace);

			context.AddChild(el);

			return el;
		}

		[OverloadResolutionPriority(1)]
		public Element C(string name, Action<Element> factory)
			=> context.C(name, context.Namespace, factory);

		[OverloadResolutionPriority(2)]
		public Element C(string name, string? ns, Action<Element> factory)
		{
			ArgumentNullException.ThrowIfNull(factory);

			var el = new Element(name, ns ?? context.Namespace);

			context.AddChild(el);

			factory(el);

			return context;
		}

		public Element Up()
		{
			return context._parent ?? context;
		}

		public Element Root()
		{
			var scope = context;

			while (scope._parent != null)
				scope = scope._parent;

			return scope;
		}
	}
}
