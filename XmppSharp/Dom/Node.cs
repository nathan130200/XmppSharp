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

public class Document
{
	private Element m_Root;
	private List<Node> m_ChildNodes = new();

	public Element RootElement
		=> m_Root;

	public IReadOnlyList<Node> ChildNodes
		=> m_ChildNodes.AsReadOnly();

	public void Load(string xml, int bufferSize = DEFAULT_BUFFER_SIZE)
	{
		using (var ms = new MemoryStream(xml.GetBytes()))
			Load(ms);
	}

	const int DEFAULT_BUFFER_SIZE = 4096;

	public void Load(Stream stream, int bufferSize = DEFAULT_BUFFER_SIZE)
	{
		if (bufferSize < 1)
			bufferSize = DEFAULT_BUFFER_SIZE;

		var parsing = true;

		using var tokenizer = new XmppTokenizer();

		byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
		int count;

		while (parsing)
		{
			if ((count = stream.Read(buffer)) <= 0)
				break;


		}

		if (m_Root == null)
			throw new InvalidOperationException("The XML document does not contain a root element.");
	}
}