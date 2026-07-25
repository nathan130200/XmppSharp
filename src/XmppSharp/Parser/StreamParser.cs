using System.Diagnostics;
using Expat;
using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Serialization;

namespace XmppSharp.Parser;

public delegate void StreamStartEventHandler(StreamStream e);

public delegate void StreamElementEventHandler(Element e);

public delegate void StreamEndEventHandler();

public sealed class StreamParser : IDisposable
{
	public event StreamStartEventHandler? OnStreamStart;

	public event StreamElementEventHandler? OnStreamElement;

	public event StreamEndEventHandler? OnStreamEnd;

	readonly Lock _syncRoot = new();

	readonly XmlParser _parser;

	readonly NamespaceStack _ns;

	volatile bool _disposed;

	Element? _context;

	int _depth;

	static readonly XmlParserOptions s_DefaultXmlOptions = new(XmlParserOptions.Default)
	{
		BillionLaughsAttackProtectionMaximumAmplification = 50f, // half percent
		BillionLaughsAttackProtectionActivationThreshold = (4 * 1024 * 1024) // 4MiB
	};

	public StreamParser(XmlParserOptions? options = default)
	{
		_ns = new NamespaceStack();

		_parser = new XmlParser(options ?? s_DefaultXmlOptions);

		_parser.OnStartElement += (name, attrs) =>
		{
			_depth++;

			_ns.PushScope();

			AddNamespacesInScope(attrs);

			if (name == "stream:stream")
			{
				var el = new StreamStream();

				foreach (var (key, value) in attrs)
					el.SetAttribute(key, value);

				OnStreamStart?.Invoke(el);
			}
			else
			{
				var element = ConstructElement(name);

				foreach (var (key, value) in attrs)
					element.SetAttribute(key, value);

				_context?.AddChild(element);

				_context = element;
			}
		};

		Element ConstructElement(string name)
		{
			var ofs = name.IndexOf(':');

			var prefix = ofs > 0 ? name[0..ofs] : string.Empty;

			var uri = _ns.LookupNamespace(prefix);

			return ElementFactory.CreateElement(name, uri);
		}

		_parser.OnEndElement += (name) =>
		{
			_depth--;

			_ns.PopScope();

			if (name == "stream:stream")
			{
				OnStreamEnd?.Invoke();
				return;
			}

			Debug.Assert(_context != null);

			var parent = _context.Parent;

			if (parent is null)
				OnStreamElement?.Invoke(_context);

			_context = parent;
		};

		_parser.OnText += (value) =>
		{
			if (_context != null)
			{
				if (_context.LastNode is Text node)
					node.Value += value;
				else
					_context.AddChild(new Text(value));
			}
		};
	}

	void AddNamespacesInScope(IReadOnlyDictionary<string, string> attrs)
	{
		foreach (var (key, value) in attrs)
		{
			if (key == "xmlns")
				_ns.AddNamespace(string.Empty, value);
			else if (key.StartsWith("xmlns:"))
			{
				var ofs = key.IndexOf(':');

				_ns.AddNamespace(key[(ofs + 1)..], value);
			}
		}
	}

	public void Reset()
	{
		lock (_syncRoot)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			_depth = 0;

			_context = null;

			_parser.Reset();

			while (_ns.PopScope()) ;
		}
	}

	public void Suspend(bool resumable = true)
	{
		lock (_syncRoot)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			_parser.Suspend(resumable);
		}
	}

	public void Resume()
	{
		lock (_syncRoot)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			_parser.Resume();
		}
	}

	public void Parse(ReadOnlySpan<byte> bytes)
	{
		lock (_syncRoot)
		{
			ObjectDisposedException.ThrowIf(_disposed, this);

			_parser.Parse(bytes);
		}
	}

	public void Dispose()
	{
		lock (_syncRoot)
		{
			if (!_disposed)
			{
				_disposed = true;

				_parser.Dispose();

				while (_ns.PopScope()) ;

				_context = null;
			}
		}
	}
}
