using System.Xml;
using System.Xml.Schema;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parser;

/// <summary>
/// An default XMPP parser implemented on top of <see cref="XmlReader"/>.
/// </summary>
public class XmppStreamReader : XmppParser
{
	private XmlReader _reader;
	private NameTable _nameTable = new();
	private volatile bool _disposed;

	private readonly bool _leaveOpen;
	private readonly bool _isFromFactory;
	private Func<Stream> _streamFactory;
	private Stream _baseStream;

	/// <summary>
	/// Initializes a new instance of <see cref="XmppStreamReader" />. Use this constructor for generic purposes, where the base type of the stream will not change (eg: loading from file).
	/// </summary>
	/// <param name="stream">Stream that will be used to read the characters.</param>
	/// <param name="leaveOpen">Determines whether the stream should remain open after dispose this parser.</param>
	public XmppStreamReader(Stream stream, bool leaveOpen = true)
	{
		Require.NotNull(stream);

		this._isFromFactory = false;
		this._leaveOpen = leaveOpen;
		this._baseStream = stream;
	}

	/// <summary>
	/// Initializes a new instance of <see cref="XmppStreamReader" />. Use this constructor only if the stream can change according to the connection state (eg: connection upgrade from raw stream to ssl stream).
	/// </summary>
	/// <param name="streamFactory">Factory function to get the stream when <see cref="Reset" /> is called.</param>
	/// <param name="leaveOpen">Determines whether the stream should remain open after dispose this parser.</param>
	public XmppStreamReader(Func<Stream> streamFactory, bool leaveOpen = true)
	{
		Require.NotNull(streamFactory);

		this._isFromFactory = true;
		this._leaveOpen = leaveOpen;
		this._streamFactory = streamFactory;
	}

	protected override void Release()
	{
		if (this._disposed)
			return;

		this._disposed = true;

		if (!this._leaveOpen)
			this._baseStream?.Dispose();

		this._baseStream = null;
		this._streamFactory = null;

		this._reader?.Dispose();
		this._reader = null;

		this._nameTable = null;
	}

	/// <summary>
	/// Restarts (or initialize) the state of the XML parser.
	/// </summary>
	/// <exception cref="ObjectDisposedException">If this instance of <see cref="XmppStreamReader" /> has already been disposed.</exception>
	public virtual void Reset()
	{
		this._reader?.Dispose();

		ThrowIfDisposed();

		if (this._isFromFactory)
			this._baseStream = this._streamFactory();

		this._reader = XmlReader.Create(this._baseStream, new()
		{
			CloseInput = false,
			Async = true,
			IgnoreWhitespace = true,
			IgnoreProcessingInstructions = true,
			ConformanceLevel = ConformanceLevel.Fragment,

			// More info: https://en.wikipedia.org/wiki/Billion_laughs_attack
			DtdProcessing = DtdProcessing.Ignore,
			XmlResolver = XmlResolver.ThrowingResolver,
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

	public virtual bool Advance()
		=> AsyncHelper.RunSync(AdvanceAsync);

	public virtual async Task<bool> AdvanceAsync()
	{
		if (this._disposed)
			return false;

		if (this._reader == null)
			return false;

		bool result;

		try
		{
			result = await this._reader.ReadAsync();
		}
		catch (XmlException e)
		{
			if (_reader.EOF)
				return false;

			throw new JabberStreamException(StreamErrorCondition.InvalidXml, e);
		}

		if (!result)
			return false;

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
						if (this._reader.NamespaceURI != Namespaces.Stream)
							throw new JabberStreamException(StreamErrorCondition.InvalidNamespace);

						await FireStreamStart((StreamStream)currentElem);
					}
					else
					{
						if (this._reader.IsEmptyElement)
						{
							if (this._rootElem != null)
								this._rootElem.AddChild(currentElem);
							else
								await FireStreamElement(currentElem);
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
						await FireStreamEnd();
					else
					{
						if (this._rootElem == null)
							throw new JabberStreamException(StreamErrorCondition.InvalidXml, "Unexcepted end tag.");

						var parent = this._rootElem.Parent;

						if (parent == null)
							await FireStreamElement(this._rootElem);

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
		}

		return result;
	}
}
