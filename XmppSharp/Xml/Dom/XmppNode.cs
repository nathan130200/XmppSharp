using System.Diagnostics;
using System.Xml;

namespace XmppSharp.Dom;

public abstract class XmppNode : ICloneable
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal XmppElement? _parent;

	public abstract XmppNode Clone();

	object ICloneable.Clone() => Clone();

	public XmppElement? Parent
	{
		get => _parent;
		set
		{
			_parent?.RemoveChild(this);
			value?.AddChild(this);
		}
	}

	public void Remove()
		=> _parent?.RemoveChild(this);

	public static string operator +(XmppNode left, object right) => string.Concat(left, right);

	public abstract void WriteTo(XmlWriter writer);
}
