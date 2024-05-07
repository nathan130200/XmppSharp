using Expat;
using XmppSharp.Dom;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Parsers;

/// <summary>
/// An enhanced XMPP parser built using Expat library.
/// </summary>
public class ExpatXmppParser : BaseXmppParser
{
	private Parser _parser;
	private Element _currentElem;

	public ExpatXmppParser(ExpatEncodingType encoding = ExpatEncodingType.Utf8)
	{
		this._parser = new Parser(encoding);

		this._parser.OnElementStart += e =>
		{
			var entity = Xml.ExtractQualifiedName(e.Name);

			string ns;

			if (entity.HasPrefix)
				e.Attributes.TryGetValue($"xmlns:{entity.Prefix}", out ns);
			else
				e.Attributes.TryGetValue("xmlns", out ns);

			if (e.Name is "iq" or "message" or "presence") // work-around
				ns ??= Namespace.Client;

			var element = ElementFactory.Create(e.Name, ns);

			if (e.Name == "stream:stream")
				AsyncHelper.RunSync(() => FireStreamStart(element as StreamStream));
			else
			{
				_currentElem?.AddChild(element);
				_currentElem = element;
			}
		};

		this._parser.OnElementEnd += e =>
		{
			if (e.Value == "stream:stream")
				AsyncHelper.RunSync(() => FireStreamEnd());
			else
			{
				var parent = _currentElem.Parent;

				if (parent == null)
					AsyncHelper.RunSync(() => FireStreamElement(_currentElem));
				else
				{
					if (e.Value != _currentElem.TagName)
					{
						var ex = new JabberStreamException(StreamErrorCondition.InvalidXml, "Parent end tag mismatch.");
						ex.Data.Add("Actual", e.Value);
						ex.Data.Add("Expected", _currentElem.TagName);
						throw ex;
					}
				}

				_currentElem = parent;
			}
		};

		this._parser.OnText += e =>
		{
			if (_currentElem != null)
			{
				if (_currentElem.LastNode is Text text)
					text.Value += e.Value;
				else
					_currentElem.AddChild(new Text(e.Value));
			}
		};

		this._parser.OnCdata += e =>
		{
			this._currentElem?.AddChild(new Cdata(e.Value));
		};

		this._parser.OnComment += e =>
		{
			this._currentElem?.AddChild(new Comment(e.Value));
		};
	}

	public void Reset()
	{
		this.EnsureNotDisposed();
		this._parser.Reset();
	}

	public void Write(byte[] buffer, int offset = 0, int length = -1, bool isFinalBlock = false)
	{
		length = length < 0 ? buffer.Length : length;
		Write(buffer[offset..length], isFinalBlock);
	}

	public void Write(byte[] buffer, bool isFinalBlock = false)
		=> this._parser.Feed(buffer, buffer.Length, isFinalBlock);

	protected override void OnDispose()
	{
		this._parser?.Dispose();
		this._parser = null;
	}
}
