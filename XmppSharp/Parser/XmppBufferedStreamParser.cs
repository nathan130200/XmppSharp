﻿// Copyright (c) 2003-2008 by AG-Software

using System.Text;
using System.Xml;
using XmppSharp.Exceptions;
using XmppSharp.Factory;
using XmppSharp.Protocol.Base;
using XmppSharp.XpNet;
using TOK = XmppSharp.XpNet.TokenType;

namespace XmppSharp.Parser;

/// <summary>XMPP parser implemented based on original agsXMPP parser (that uses JavaXP port to .NET)</summary>
public class XmppBufferedStreamParser : XmppParser
{
	static readonly UTF8Encoding s_UTF8 = new(false, true);
	private XmlEncoding _enc = new UTF8XmlEncoding();
	private XmlNamespaceManager _namespaceMgr;

	private NameTable _stringPool = new();
	private BufferAggregate _buf;
	private bool _isCdata = false;
	private StringBuilder _cdataBuffer = new();
	private Element _current;

	public XmppBufferedStreamParser()
	{
		_namespaceMgr = new XmlNamespaceManager(_stringPool);
		Reset();
	}

	protected override void Release()
	{
		_cdataBuffer.Clear();
		_cdataBuffer = null;
		_stringPool = null;
		_buf?.Dispose();
		_buf = null;

		while (_namespaceMgr.PopScope())
			break;

		_namespaceMgr = null;
		_enc = null;

		_current = null;
	}

	public void Reset()
	{
		ThrowIfDisposed();

		_current = null;
		_isCdata = false;
		_cdataBuffer.Clear();
		_buf?.Dispose();
		_buf = new();
	}

	public void Write(byte[] buf)
		=> Write(buf, buf.Length);

	public void Write(byte[] buf, int count)
	{
		ThrowIfDisposed();

		if (count <= 0)
			return;

		lock (this)
		{
			var temp = new byte[count];
			Array.ConstrainedCopy(buf, 0, temp, 0, count);
			_buf.Write(temp);

			WriteInternal();
		}
	}

	void WriteInternal()
	{
		var b = _buf.GetBuffer();
		int off = 0;

		var tok = TOK.END_TAG;
		ContentToken ct = new();

		try
		{
			while (off < b.Length)
			{
				if (_isCdata)
					tok = _enc.TokenizeCdataSection(b, off, b.Length, ct);
				else
					tok = _enc.TokenizeContent(b, off, b.Length, ct);

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
						if (_current != null)
						{
							// <!-- 4
							//  --> 3
							int start = off + 4 * _enc.MinBytesPerChar;
							int end = ct.TokenEnd - off -
								7 * _enc.MinBytesPerChar;
							string text = s_UTF8.GetString(b, start, end);
							_current.AddChild(new Comment(text));
						}
						break;
					case TOK.CDATA_SECT_OPEN:
						_isCdata = true;
						_cdataBuffer.Clear();
						break;
					case TOK.CDATA_SECT_CLOSE:

						if (_cdataBuffer.Length > 0)
						{
							var content = _cdataBuffer.ToString();
							_current?.AddChild(new Cdata(content));
						}
						_isCdata = false;
						_cdataBuffer.Clear();

						break;

					case TOK.XML_DECL:
						break;

					case TOK.ENTITY_REF:
					case TOK.PI:
						throw new JabberStreamException(StreamErrorCondition.NotWellFormed);
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

	private string NormalizeAttributeValue(byte[] buf, int offset, int length)
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
				tok = _enc.TokenizeAttributeValue(b, off, b.Length, ct);

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
						throw new JabberStreamException(StreamErrorCondition.NotWellFormed);
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

	void StartTag(byte[] buf, int offset, ContentToken ct, TOK tok)
	{
		int colon;
		string name;
		string prefix;
		var ht = new Dictionary<string, string>();

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
				//val = utf.GetString(buf, start, end - start);

				val = NormalizeAttributeValue(buf, start, end - start);

				if (name.StartsWith("xmlns:"))
				{
					colon = name.IndexOf(':');
					prefix = name.Substring(colon + 1);
					_namespaceMgr.AddNamespace(prefix, val);
					ht[name] = _stringPool.Add(val);
				}
				else if (name == "xmlns")
				{
					_namespaceMgr.AddNamespace(string.Empty, val);
					ht[name] = _stringPool.Add(val);
				}
				else
				{
					ht[name] = val;
				}
			}
		}

		name = _stringPool.Add(s_UTF8.GetString(buf,
			offset + _enc.MinBytesPerChar,
			ct.NameEnd - offset - _enc.MinBytesPerChar));

		colon = name.IndexOf(':');
		string ns;

		if (colon > 0)
		{
			prefix = name.Substring(0, colon);
			ns = _namespaceMgr.LookupNamespace(prefix);
		}
		else
		{
			ns = _namespaceMgr.DefaultNamespace;
		}

		// eg: When session sends a stanza without namespace. Fallback to `jabber:client` instead.

		if (name is "iq" or "message" or "presence" && string.IsNullOrEmpty(ns))
			ns = Namespaces.Client;

		Element newElement = ElementFactory.Create(name, ns);

		foreach (var (key, val) in ht)
			newElement.SetAttribute(key, val);

		if (name == "stream:stream")
			AsyncHelper.RunSync(() => this.FireStreamStart(newElement as StreamStream));
		else
		{
			_current?.AddChild(newElement);
			_current = newElement;
		}
	}

	protected virtual void EndTag(byte[] buf, int offset, ContentToken ct, TOK tok)
	{
		_namespaceMgr.PopScope();

		string name = null;

		if ((tok == TOK.EMPTY_ELEMENT_WITH_ATTS) ||
			(tok == TOK.EMPTY_ELEMENT_NO_ATTS))
			name = s_UTF8.GetString(buf,
				offset + _enc.MinBytesPerChar,
				ct.NameEnd - offset -
				_enc.MinBytesPerChar);
		else
			name = s_UTF8.GetString(buf,
				offset + _enc.MinBytesPerChar * 2,
				ct.NameEnd - offset -
				_enc.MinBytesPerChar * 2);

		name = _stringPool.Add(name);

		if (name == "stream:stream")
			AsyncHelper.RunSync(FireStreamEnd);
		else
		{
			if (_current == null)
				throw new JabberStreamException(StreamErrorCondition.InternalServerError, "Unexcepted null child element.");

			var parent = _current.Parent;

			if (parent == null)
				AsyncHelper.RunSync(() => FireStreamElement(_current));

			_current = parent;
		}
	}

	protected virtual void AddText(string text)
	{
		if (_current == null)
			return;

		if (_isCdata)
			_cdataBuffer.Append(text);
		else
		{
			if (_current.LastNode is Text t)
				t.Value += text;
			else
				_current.AddChild(new Text(text));
		}
	}
}
