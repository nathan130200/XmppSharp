﻿using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using XmppSharp.XpNet;
using TOK = XmppSharp.XpNet.TokenType;

namespace XmppSharp;

public delegate void StartElementDelegate(XmlNameInfo name, IReadOnlyDictionary<XmlNameInfo, string> attrs);
public delegate void EndElementDelegate(XmlNameInfo name);
public delegate void ContentDelegate(string value);

public class XmppTokenizer : IDisposable
{
	private volatile bool _disposed;

	static readonly UTF8Encoding s_UTF8 = new(false, false);
	static readonly UTF8XmlEncoding s_xUTF8 = new();

	private NameTable _stringPool = new();
	private BufferAggregate _buf;
	private bool _isCdata = false;
	private XmlNamespaceManager _namespaceMgr;
	private StringBuilder _cdata = new();

	public event StartElementDelegate OnElementStart;
	public event EndElementDelegate OnElementEnd;
	public event ContentDelegate OnText;
	public event ContentDelegate OnCdata;
	public event ContentDelegate OnComment;

	public XmppTokenizer()
	{
		_namespaceMgr = new XmlNamespaceManager(_stringPool);
		Reset();
	}

	protected virtual void Disposing()
	{

	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ThrowIfDisposed()
		=> ObjectDisposedException.ThrowIf(_disposed, this);

	public void Dispose()
	{
		if (_disposed)
			return;

		_disposed = true;

		Disposing();

		_cdata.Clear();
		_cdata = null;

		_buf?.Dispose();
		_buf = null;

		while (_namespaceMgr.PopScope())
			;

		_namespaceMgr = null;

		_stringPool = null;

		GC.SuppressFinalize(this);
	}

	public void Reset()
	{
		ThrowIfDisposed();

		_isCdata = false;
		_cdata.Clear();
		_buf?.Dispose();
		_buf = new();
	}

	public void Write(byte[] buf)
		=> Write(buf, buf.Length);

	public virtual void Write(byte[] buf, int count)
	{
		ThrowIfDisposed();

		if (count <= 0)
			return;

		lock (this)
		{
			var temp = new byte[count];
			Array.ConstrainedCopy(buf, 0, temp, 0, count);
			_buf.Write(temp);
			Parse();
		}
	}

	// Maybe someone need override this method?
	protected virtual void Parse()
	{
		var b = _buf.GetBuffer();
		int off = 0;

#pragma warning disable
		TOK tok = TOK.END_TAG;
#pragma warning restore

		ContentToken ct = new();

		try
		{
			while (off < b.Length)
			{
				if (_isCdata)
					tok = s_xUTF8.TokenizeCdataSection(b, off, b.Length, ct);
				else
					tok = s_xUTF8.TokenizeContent(b, off, b.Length, ct);

				switch (tok)
				{
					case TOK.EMPTY_ELEMENT_NO_ATTS:
					case TOK.EMPTY_ELEMENT_WITH_ATTS:
						StartTag(b, off, ct, tok);
						EndTag(b, off, ct, tok);
						break;
					case TOK.START_TAG_NO_ATTS:
					case TOK.START_TAG_WITH_ATTS:
						StartTag(b, off, ct, tok);
						break;
					case TOK.END_TAG:
						EndTag(b, off, ct, tok);
						break;
					case TOK.DATA_CHARS:
					case TOK.DATA_NEWLINE:
						AddText(s_UTF8.GetString(b, off, ct.TokenEnd - off));
						break;
					case TOK.CHAR_REF:
					case TOK.MAGIC_ENTITY_REF:
						AddText(new string(new char[] { ct.RefChar1 }));
						break;
					case TOK.CHAR_PAIR_REF:
						AddText(new string(new char[] {ct.RefChar1,
															ct.RefChar2}));
						break;
					case TOK.COMMENT:
						{
							// <!-- 4
							//  --> 3
							int start = off + 4 * s_xUTF8.MinBytesPerChar;
							int end = ct.TokenEnd - off -
								7 * s_xUTF8.MinBytesPerChar;
							string text = s_UTF8.GetString(b, start, end);

							OnComment?.Invoke(text);
						}
						break;
					case TOK.CDATA_SECT_OPEN:
						_isCdata = true;
						_cdata.Clear();
						break;
					case TOK.CDATA_SECT_CLOSE:

						if (_cdata.Length > 0)
						{
							var content = _cdata.ToString();
							_cdata.Clear();
							OnCdata?.Invoke(content);
						}
						_isCdata = false;
						_cdata.Clear();

						break;

					case TOK.XML_DECL:
					case TOK.PI:
						break;

					case TOK.ENTITY_REF:
						throw new XmlException("XML not well formed.");
				}

				off = ct.TokenEnd;
			}
		}
		catch (PartialTokenException) { }
		catch (ExtensibleTokenException) { }
		catch (Exception)
		{
			throw;
		}
		finally
		{
			_buf.Clear(off);
		}
	}

	protected virtual string NormalizeAttributeValue(byte[] buf, int offset, int length)
	{
		if (length == 0)
			return null;

		string val = null;
		using var buffer = new BufferAggregate();

		byte[] copy = new byte[length];
		Buffer.BlockCopy(buf, offset, copy, 0, length);
		buffer.Write(copy);

		byte[] b = buffer.GetBuffer();
		int off = 0;

		var tok = TOK.END_TAG;
		var ct = new ContentToken();
		try
		{
			while (off < b.Length)
			{
				tok = s_xUTF8.TokenizeAttributeValue(b, off, b.Length, ct);

				switch (tok)
				{
					case TOK.ATTRIBUTE_VALUE_S:
					case TOK.DATA_CHARS:
					case TOK.DATA_NEWLINE:
						val += (s_UTF8.GetString(b, off, ct.TokenEnd - off));
						break;
					case TOK.CHAR_REF:
					case TOK.MAGIC_ENTITY_REF:
						val += new string(new char[] { ct.RefChar1 });
						break;
					case TOK.CHAR_PAIR_REF:
						val += new string(new char[] { ct.RefChar1, ct.RefChar2 });
						break;
					case TOK.ENTITY_REF:
						throw new XmlException("XML not well formed.");
				}

				off = ct.TokenEnd;
			}
		}
		catch (PartialTokenException) { }
		catch (ExtensibleTokenException) { }
		catch (Exception)
		{
			throw;
		}
		finally
		{
			buffer.Clear(off);
		}

		return val;
	}

	public virtual string? LookupNamespace(string? prefix = default)
		=> _namespaceMgr.LookupNamespace(prefix ?? string.Empty);

	public virtual string? LookupPrefix(string namespaceURI)
		=> _namespaceMgr.LookupPrefix(namespaceURI);

	protected virtual void StartTag(byte[] buf, int offset, ContentToken ct, TOK tok)
	{
		int colon;
		string name;
		string? prefix = default;

		var attributes = new Dictionary<XmlNameInfo, string>();

		_namespaceMgr.PushScope();

		if (tok == TOK.START_TAG_WITH_ATTS || tok == TOK.EMPTY_ELEMENT_WITH_ATTS)
		{
			int start;
			int end;
			string val;

			for (int i = 0; i < ct.GetAttributeSpecifiedCount(); i++)
			{
				start = ct.GetAttributeNameStart(i);
				end = ct.GetAttributeNameEnd(i);
				name = _stringPool.Add(s_UTF8.GetString(buf, start, end - start));

				start = ct.GetAttributeValueStart(i);
				end = ct.GetAttributeValueEnd(i);

				val = NormalizeAttributeValue(buf, start, end - start);

				if (name.StartsWith("xmlns:"))
				{
					colon = name.IndexOf(':');
					prefix = name.Substring(colon + 1);
					_namespaceMgr.AddNamespace(prefix, val ?? string.Empty);
				}
				else if (name == "xmlns")
					_namespaceMgr.AddNamespace(string.Empty, val ?? string.Empty);

				attributes[name] = val;
			}
		}

		name = _stringPool.Add(s_UTF8.GetString(buf,
			offset + s_xUTF8.MinBytesPerChar,
			ct.NameEnd - offset - s_xUTF8.MinBytesPerChar));

		OnElementStart?.Invoke(name, attributes);
	}

	protected virtual void EndTag(byte[] buf, int offset, ContentToken ct, TOK tok)
	{
		_namespaceMgr.PopScope();

		string name;

		if ((tok == TOK.EMPTY_ELEMENT_WITH_ATTS) ||
			(tok == TOK.EMPTY_ELEMENT_NO_ATTS))
			name = s_UTF8.GetString(buf,
				offset + s_xUTF8.MinBytesPerChar,
				ct.NameEnd - offset -
				s_xUTF8.MinBytesPerChar);
		else
			name = s_UTF8.GetString(buf,
				offset + s_xUTF8.MinBytesPerChar * 2,
				ct.NameEnd - offset -
				s_xUTF8.MinBytesPerChar * 2);

		name = _stringPool.Add(name);

		OnElementEnd?.Invoke(name);
	}

	protected virtual string NormalizeCdataContent(string text)
	{
		return text;
	}

	protected virtual string NormalizeTextContent(string text)
	{
		return text;
	}

	protected virtual void AddText(string text)
	{
		if (_isCdata)
			_cdata.Append(NormalizeCdataContent(text));
		else
			OnText?.Invoke(NormalizeTextContent(text));
	}
}
