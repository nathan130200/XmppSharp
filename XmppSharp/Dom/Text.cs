using System.Diagnostics;
using System.Text;
using System.Xml;
using XmppSharp.Dom.Abstractions;

namespace XmppSharp.Dom;

/// <summary>Represents text content node.</summary>
public sealed class Text : Node
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	string? _value;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	StringBuilder? _builder;

	/// <summary>Initialize new instance of <see cref="Text"/>.</summary>
	public Text()
	{
		_value = string.Empty;
	}

	/// <summary>Initialize new instance of <see cref="Text"/>.</summary>
	/// <param name="value">Initial text content.</param>
	public Text(string? value)
	{
		_value = value ?? string.Empty;
	}

	/// <summary>Gets the string content.</summary>
	public string Value
	{
		get
		{
			if (_value is not null)
				return _value;

			_value = _builder!.ToString();
			_builder = null;

			return _value;
		}
	}

	/// <summary>Append content to this node</summary>
	/// <param name="value">Content to add.</param>
	public void Append(ReadOnlySpan<char> value)
	{
		if (value.IsEmpty)
			return;

		if (_builder is null)
		{
			_builder = new StringBuilder(_value);
			_value = null;
		}

		_builder.Append(value);
	}

	/// <summary>Append UTF-8 content to this node</summary>
	/// <param name="bytes">Content to add.</param>
	public void AppendUtf8(ReadOnlySpan<byte> bytes)
	{
		var numChars = Encoding.UTF8.GetCharCount(bytes);

		Span<char> span = numChars < 256
			? stackalloc char[numChars]
			: new char[numChars];

		Encoding.UTF8.GetChars(bytes, span);

		Append(span);
	}

	/// <inheritdoc/>
	public override Node Clone()
	{
		if (_builder is null)
			return new Text(_value);

		return new Text
		{
			_value = null,
			_builder = new StringBuilder(_builder.ToString())
		};
	}

	/// <inheritdoc/>
	public override void WriteTo(XmlWriter writer)
	{
		writer.WriteString(Value);
	}

	/// <inheritdoc/>
	public override string ToString() => Value;
}