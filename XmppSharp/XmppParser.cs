using System.Text;
using System.Xml;
using System.Xml.Schema;

/* Unmerged change from project 'XmppSharp (net6.0)'
Before:
using XmppSharp.Exceptions;
After:
using XmppSharp;
using XmppSharp;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
*/
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp;

public sealed class XmppParser : IDisposable
{
	private XmlReader _reader;
	private NameTable _nameTable = new();
	private volatile bool _disposed;

	private readonly Encoding _encoding;
	private readonly int _bufferSize;

	public const int DefaultBufferSize = 256;

	public XmppParser(Encoding? encoding = default, int bufferSize = DefaultBufferSize)
	{
		this._encoding = encoding ?? Encoding.UTF8;
		this._bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
	}

	/// <summary>
	/// The event is triggered when the XMPP open tag is found <c>&lt;stream:stream&gt;</c>
	/// </summary>
	public event AsyncAction<StreamStream> OnStreamStart;

	/// <summary>
	/// The event is triggered when any well-formed element is found.
	/// <para>However, if the XML tag is registered using <see cref="ElementFactory" /> the parser will automatically construct the element in the registered type.</para>
	/// <para>Elements that cannot be constructed using <see cref="ElementFactory" /> only return element as base type of <see cref="Element" />.</para>
	/// </summary>
	public event AsyncAction<Element> OnStreamElement;

	/// <summary>
	/// The event is triggered when the XMPP close tag is found <c>&lt;/stream:stream&gt;</c>
	/// </summary>
	public event AsyncAction OnStreamEnd;

	/// <inheritdoc/>
	public void Dispose()
	{
		if (this._disposed)
			return;

		this._disposed = true;

		this._reader?.Dispose();
		this._nameTable = null;
	}

#if !NET7_0_OR_GREATER

	internal class ThrowingResolver : XmlResolver
	{
		public static ThrowingResolver Shared { get; } = new();

		public override object? GetEntity(Uri absoluteUri, string? role, Type? ofObjectToReturn)
		{
			throw new NotSupportedException($"Unable to resolve XML entity: {absoluteUri} ({role})");
		}
	}

#endif

	/// <summary>
	/// Restarts the internal state of the XML parser.
	/// </summary>
	/// <param name="stream">Stream that will be used to read the characters.</param>
	/// <exception cref="ObjectDisposedException">If this instance of <see cref="XmppParser" /> has already been disposed.</exception>
	public void Reset(Stream stream)
	{
		this._reader?.Dispose();

#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(this._disposed, this);
#else
		if (this._disposed)
			throw new ObjectDisposedException(this.GetType().FullName);
#endif
		this._reader = XmlReader.Create(new StreamReader(stream, this._encoding, false, this._bufferSize, true), new()
		{
			CloseInput = true,
			Async = true,
			IgnoreProcessingInstructions = true,
			IgnoreWhitespace = true,
			ConformanceLevel = ConformanceLevel.Fragment,
			DtdProcessing = DtdProcessing.Prohibit,
#if NET7_0_OR_GREATER
			XmlResolver = XmlResolver.ThrowingResolver,
#else
			XmlResolver = ThrowingResolver.Shared,
#endif
			ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
			NameTable = this._nameTable
		});
	}

	private Element _rootElem;

	/// <summary>
	/// Gets the XML element in current scope.
	/// </summary>
	public Element? CurrentElement
		=> this._rootElem;

	/// <summary>
	/// Gets the XML depth in the parser tree.
	/// </summary>
	public int Depth
	{
		get
		{
			if (this._disposed)
				return 0;

			return this._reader?.Depth ?? 0;
		}
	}

	public async Task<bool> Advance()
	{
		if (this._disposed)
			return false;

		if (this._reader == null || this._reader != null && this._reader.EOF)
			return false;

		bool result;

		try
		{
			result = await this._reader.ReadAsync();
		}
		catch (XmlException e)
		{
			throw new JabberStreamException(StreamErrorCondition.InvalidXml, e);
		}

		if (result)
		{
			switch (this._reader.NodeType)
			{
				case XmlNodeType.Element:
					{
						Element currentElem;

						if (this._reader.Name != "stream:stream")
						{
							var ns = this._reader.NamespaceURI;

							if (string.IsNullOrEmpty(ns) && this._reader.LocalName is "iq" or "message" or "presence")
								ns = "jabber:client";

							currentElem = ElementFactory.Create(this._reader.Name, ns);
						}
						else
							currentElem = new StreamStream();

						if (this._reader.HasAttributes)
						{
							while (this._reader.MoveToNextAttribute())
								currentElem.SetAttribute(this._reader.Name, this._reader.Value);

							this._reader.MoveToElement();
						}

						if (this._reader.Name == "stream:stream")
						{
							if (this._reader.NamespaceURI != Namespace.Stream)
								throw new JabberStreamException(StreamErrorCondition.InvalidNamespace);

							await OnStreamStart.InvokeAsync((StreamStream)currentElem);
						}
						else
						{
							if (this._reader.IsEmptyElement)
							{
								if (this._rootElem != null)
									this._rootElem.AddChild(currentElem);
								else
									await OnStreamElement.InvokeAsync(currentElem);
							}
							else
							{
								this._rootElem?.AddChild(currentElem);
								this._rootElem = currentElem;
							}
						}
					}
					break;

				case XmlNodeType.EndElement:
					{
						if (this._reader.Name == "stream:stream")
							await OnStreamEnd.InvokeAsync();
						else
						{
							if (this._rootElem == null)
								throw new JabberStreamException(StreamErrorCondition.InvalidXml, "The element in the current scope was not expected to be null.");

							var parent = this._rootElem.Parent;

							if (parent == null)
								await OnStreamElement.InvokeAsync(this._rootElem);

							this._rootElem = parent;
						}
					}
					break;

				case XmlNodeType.SignificantWhitespace:
				case XmlNodeType.Text:
					{
						if (this._rootElem != null)
						{
							if (this._rootElem.LastNode is Text text)
								text.Value += this._reader.Value;
							else
								this._rootElem.AddChild(new Text(this._reader.Value));
						}
					}
					break;

				case XmlNodeType.Comment:
					this._rootElem?.AddChild(new Comment(this._reader.Value));
					break;

				case XmlNodeType.CDATA:
					this._rootElem?.AddChild(new Cdata(this._reader.Value));
					break;

				default:
					break;
			}

			return true;
		}

		return false;
	}
}
