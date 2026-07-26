using System.Diagnostics;
using System.Runtime.CompilerServices;
using XmppSharp.Dom.Abstractions;
using XmppSharp.Serialization;

namespace XmppSharp.Dom;

public static class DomExtensions
{
	public static T As<T>(this IXmlNode node) => (T)node;

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Element C(this Element parent, string name, string? ns = default, object? value = default, Action<Element>? builder = default)
	{
		ArgumentNullException.ThrowIfNull(parent);

		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

		var result = ElementFactory.CreateElement(name, ns);

		result.SetValue(value);

		if (builder != null)
		{
			try
			{
				builder(result);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		parent.AddChild(result);

		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Element C(this Element parent, string name, Action<Element>? builder)
		=> C(parent, name, null, builder);

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Element C(this Element parent, string name, string? ns, Action<Element>? builder)
	{
		ArgumentNullException.ThrowIfNull(parent);

		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

		var result = ElementFactory.CreateElement(name, ns);

		if (builder != null)
		{
			try
			{
				builder(result);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		parent.AddChild(result);

		return result;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Element C(this Element parent, Element child)
	{
		ArgumentNullException.ThrowIfNull(parent);

		ArgumentNullException.ThrowIfNull(child);

		parent.AddChild(child);

		return parent;
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Element Up(this Element e) => e.Parent ?? e;

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static Element Root(this Element e)
	{
		ArgumentNullException.ThrowIfNull(e);

		while (!e.IsRootElement)
			e = e.Parent!;

		return e;
	}
}