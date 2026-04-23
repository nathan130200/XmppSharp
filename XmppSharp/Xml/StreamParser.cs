using System.Diagnostics;
using Expat;
using XmppSharp.Dom;

namespace XmppSharp.Xml;

/// <summary>
/// Provides functionality to parse XML streams, specifically designed for XMPP stream parsing.
/// </summary>
/// <remarks>
/// <para>
/// This class uses an underlying expat parser to process XML data incrementally.
/// </para>
/// </remarks>
public class StreamParser : IDisposable
{
    Element? _currentElement;
    XmlParser? _parser;
    volatile bool _disposed;
    NamespaceStack? _namespaces;
    readonly Lock _syncRoot = new();

    /// <summary>
    /// Raised when a stream starts, i.e. when the <c>stream:stream</c> element is opened.
    /// </summary>
    public event Action<Protocol.Base.Stream>? OnStreamStart;

    /// <summary>
    /// Raised when an element within the stream is parsed.
    /// </summary>
    public event Action<Element>? OnStreamElement;

    /// <summary>
    /// Raised when a stream ends, i.e. when the <c>stream:stream</c> element is closed.
    /// </summary>
    public event Action? OnStreamEnd;

    static string? ExtractPrefix(string tag)
    {
        var ofs = tag.IndexOf(':');
        return ofs > 0 ? string.Intern(tag[0..ofs]) : null;
    }

    /// <summary>
    /// Fires the <see cref="OnStreamStart"/> event with the provided stream element. This method is called internally when the <c>stream:stream</c> element is opened during parsing.
    /// </summary>
    /// <param name="element">The stream element that has started.</param>
    protected void FireOnStreamStart(Protocol.Base.Stream element)
        => OnStreamStart?.Invoke(element);

    /// <summary>
    /// Fires the <see cref="OnStreamElement"/> event with the provided element. This method is called internally when an element within the stream is parsed.
    /// </summary>
    /// <param name="element">The element that has been parsed.</param>
    protected void FireOnStreamElement(Element element)
        => OnStreamElement?.Invoke(element);

    /// <summary>
    /// Fires the <see cref="OnStreamEnd"/> event. This method is called internally when the <c>stream:stream</c> element is closed during parsing.
    /// </summary>
    protected void FireOnStreamEnd()
        => OnStreamEnd?.Invoke();

    /// <summary>
    /// Initializes a new instance of the <see cref="StreamParser"/> class.
    /// </summary>
    /// <param name="options">
    /// The options to configure the underlying expat parser. If <c>null</c>, default options will be used.
    /// </param>
    public StreamParser(XmlParserOptions? options = null)
    {
        _namespaces = new();

        _parser = new XmlParser(options);

        _parser.OnStartTag += (name, attributes) =>
        {
            _namespaces.PushScope();

            foreach (var (key, value) in attributes)
            {
                if (key == "xmlns")
                    _namespaces.AddNamespace(string.Empty, value);
                else if (key.StartsWith("xmlns:"))
                {
                    var colonStart = key.IndexOf(':');
                    var prefix = key[(colonStart + 1)..];
                    _namespaces.AddNamespace(prefix, value);
                }
            }

            var ns = _namespaces.LookupNamespace(ExtractPrefix(name));

            var element = ElementFactory.Create(string.Intern(name), ns);

            foreach (var (key, value) in attributes)
                element.SetAttribute(string.Intern(key), value);

            if (name == "stream:stream")
                FireOnStreamStart((Protocol.Base.Stream)element);
            else
            {
                _currentElement?.AddChild(element);
                _currentElement = element;
            }
        };

        _parser.OnEndTag += name =>
        {
            _namespaces.PopScope();

            if (name == "stream:stream")
            {
                FireOnStreamEnd();
                return;
            }

            Debug.Assert(_currentElement != null);

            var parent = _currentElement.Parent;

            if (parent is null)
                FireOnStreamElement(_currentElement);

            _currentElement = parent;
        };

        _parser.OnText += value =>
        {
            if (_currentElement == null)
                return;

            if (_currentElement.LastNode is Text text)
                text.Value += value;
            else
                _currentElement.AddChild(new Text(value));
        };

        _parser.OnCdata += value =>
        {
            _currentElement?.AddChild(new Cdata(value));
        };
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="StreamParser"/> class.
    /// </summary>
    ~StreamParser()
    {
        Dispose();
    }

    /// <summary>
    /// Disposes the stream parser, releasing all resources. After calling this method, the parser cannot be used anymore.
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            lock (_syncRoot)
                _parser!.Suspend(false);

            _parser!.Dispose();

            _parser = null;

            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Resets the stream parser to its initial state, allowing it to be reused for parsing a new stream.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the parser has been disposed.</exception>
    /// <exception cref="ExpatException">Thrown if an error occurs while resuming the parser.</exception>
    public void Reset()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_syncRoot)
            _parser!.Reset();
    }


    /// <summary>
    /// Suspends the stream parser.
    /// </summary>
    /// <param name="resumable">Indicates whether the parser can be resumed after being suspended.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the parser has been disposed.</exception>
    /// <exception cref="ExpatException">Thrown if an error occurs while resuming the parser.</exception>
    public void Suspend(bool resumable = true)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_syncRoot)
            _parser!.Suspend(resumable);
    }

    /// <summary>
    /// Resumes the stream parser if it was previously suspended. If the parser was not suspended or cannot be resumed, this method has no effect.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the parser has been disposed.</exception>
    /// <exception cref="ExpatException">Thrown if an error occurs while resuming the parser.</exception>
    public void Resume()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_syncRoot)
            _parser!.Resume();
    }

    /// <summary>
    /// Parses a chunk of XML data.
    /// <para>
    /// This method can be called multiple times to parse a stream of XML data incrementally.
    /// </para>
    /// <para>
    /// If the <paramref name="isFinal"/> parameter is set to <c>true</c>, the parser will
    /// treat this chunk as the final part of the XML data, which may trigger end-of-stream events.</para>
    /// </summary>
    /// <param name="buffer">The buffer containing the XML data to parse.</param>
    /// <param name="count">The number of bytes to parse from the buffer.</param>
    /// <param name="isFinal">Indicates whether this is the final chunk of XML data.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the parser has been disposed.</exception>
    /// <exception cref="ExpatException">Thrown if an error occurs while parsing the XML data.</exception>
    public void Parse(byte[] buffer, int count, bool isFinal = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        lock (_syncRoot)
            _parser!.Parse(buffer, count, isFinal);
    }
}
