using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp;

public sealed class XmppParser : IDisposable
{
	private XmlReader _reader;
	private StreamReader _textReader;
	private NameTable _nameTable = new();
	private volatile bool _disposed;


	private readonly bool _leaveOpen;
	private readonly bool _isFromFactory;
	private Func<Stream> _streamFactory;
	private Stream _baseStream;

	private readonly Encoding _encoding;
	private readonly int _bufferSize;

	public const int DefaultBufferSize = 256;

	XmppParser(Encoding? encoding, int bufferSize)
	{
		this._encoding = encoding ?? Encoding.UTF8;
		this._bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
	}

	/// <summary>
	/// Initializes a new instance of <see cref="XmppParser" />. Use this constructor for generic purposes, where the base type of the stream will not change (eg: loading from file).
	/// </summary>
	/// <param name="stream">Stream that will be used to read the characters.</param>
	/// <param name="leaveOpen">Determines whether the stream should remain open after dispose this parser.</param>
	/// <param name="encoding">Determines which type of character encoding to be used. (Default: <see cref="Encoding.UTF8"/>)</param>
	/// <param name="bufferSize">Buffer size in chars for the internal <see cref="StreamReader" />. (Default: <see cref="DefaultBufferSize"/>)</param>
	public XmppParser(Stream stream, bool leaveOpen = true, Encoding? encoding = default, int bufferSize = -1) : this(encoding, bufferSize)
	{
		Require.NotNull(stream);

		this._isFromFactory = false;
		this._leaveOpen = leaveOpen;
		this._baseStream = stream;

		Reset();
	}

	/// <summary>
	/// Initializes a new instance of <see cref="XmppParser" />. Use this constructor only if the stream can change according to the connection state (eg: connection upgrade from raw stream to ssl stream).
	/// </summary>
	/// <param name="streamFactory">Factory function to get the stream when <see cref="Reset" /> is called.</param>
	/// <param name="encoding">Determines which type of character encoding to be used. (Default: <see cref="Encoding.UTF8"/>)</param>
	/// <param name="bufferSize">Buffer size in chars for the internal <see cref="StreamReader" />. (Default: <see cref="DefaultBufferSize"/>)</param>
	public XmppParser(Func<Stream> streamFactory, Encoding? encoding = default, int bufferSize = -1) : this(encoding, bufferSize)
	{
		Require.NotNull(streamFactory);

		this._isFromFactory = true;
		this._streamFactory = streamFactory;

		Reset();
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

		if (this._isFromFactory)
			this._streamFactory = null;
		else
		{
			if (!this._leaveOpen)
				this._baseStream?.Dispose();

			this._baseStream = null;
		}

		this._reader?.Dispose();
        this._textReader?.Dispose();
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
	/// Restarts the state of the XML parser.
	/// </summary>
	/// <exception cref="ObjectDisposedException">If this instance of <see cref="XmppParser" /> has already been disposed.</exception>
	public void Reset()
	{
		this._reader?.Dispose();
		this._textReader?.Dispose();

#if NET7_0_OR_GREATER
		ObjectDisposedException.ThrowIf(this._disposed, this);
#else
		if (this._disposed)
			throw new ObjectDisposedException(GetType().FullName, "Cannot reset parser in a disposed parser.");
#endif
		this._textReader = new StreamReader(this._isFromFactory
			? this._streamFactory()
			: this._baseStream, this._encoding, false, this._bufferSize, true);

		this._reader = XmlReader.Create(this._textReader, new()
		{
			CloseInput = false,
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

	public bool Advance()
		=> AdvanceAsync().GetAwaiter().GetResult();

	public async Task<bool> AdvanceAsync()
	{
		if (this._disposed)
			return false;

        if (this._reader == null)
            return false;

        if (this._reader.EOF)
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
