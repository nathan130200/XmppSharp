using System.Buffers;
using System.Diagnostics;
using System.Xml;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Dom;

public abstract class Node : ICloneable
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal Element _parent;

	public Element Parent
	{
		get => this._parent;
		set
		{
			this._parent?.RemoveChild(this);
			value?.AddChild(this);
			_parent = value;
		}
	}

	public virtual void Remove()
	{
		this._parent?.RemoveChild(this);
		this._parent = null;
	}

	public virtual string Value
	{
		get;
		set;
	}

	public abstract void WriteTo(XmlWriter writer, in XmlFormatting formatting);
	public abstract Node Clone();

	object ICloneable.Clone() => this.Clone();
}