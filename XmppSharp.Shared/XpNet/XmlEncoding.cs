#pragma warning disable

// Copyright (c) 2003-2008 by AG-Software

namespace XmppSharp.XpNet;

using TOK = TokenType;

public abstract class XmlEncoding
{
	// Bytes with type < 0 may not be data in content.
	// The negation of the lead byte type gives the total number of bytes.

	/// <summary>
	/// Need more bytes
	/// </summary>
	protected const int BT_LEAD2 = -2;
	/// <summary>
	/// Need more bytes
	/// </summary>
	protected const int BT_LEAD3 = -3;
	/// <summary>
	/// Need more bytes
	/// </summary>
	protected const int BT_LEAD4 = -4;
	/// <summary>
	/// Not XML
	/// </summary>
	protected const int BT_NONXML = BT_LEAD4 - 1;
	/// <summary>
	/// Malformed XML
	/// </summary>
	protected const int BT_MALFORM = BT_NONXML - 1;
	/// <summary>
	/// Less than
	/// </summary>
	protected const int BT_LT = BT_MALFORM - 1;
	/// <summary>
	/// Ampersand
	/// </summary>
	protected const int BT_AMP = BT_LT - 1;
	/// <summary>
	/// right square bracket
	/// </summary>
	protected const int BT_RSQB = BT_AMP - 1;
	/// <summary>
	/// carriage return
	/// </summary>
	protected const int BT_CR = BT_RSQB - 1;
	/// <summary>
	/// line feed
	/// </summary>
	protected const int BT_LF = BT_CR - 1;

	// Bytes with type >= 0 are treated as data in content.

	/// <summary>
	/// greater than
	/// </summary>
	protected const int BT_GT = 0;
	/// <summary>
	/// Quote
	/// </summary>
	protected const int BT_QUOT = BT_GT + 1;
	/// <summary>
	/// Apostrophe
	/// </summary>
	protected const int BT_APOS = BT_QUOT + 1;
	/// <summary>
	/// Equal sign
	/// </summary>
	protected const int BT_EQUALS = BT_APOS + 1;
	/// <summary>
	/// Question mark
	/// </summary>
	protected const int BT_QUEST = BT_EQUALS + 1;
	/// <summary>
	/// Exclamation point
	/// </summary>
	protected const int BT_EXCL = BT_QUEST + 1;
	/// <summary>
	/// Solidus (/)
	/// </summary>
	protected const int BT_SOL = BT_EXCL + 1;
	/// <summary>
	/// Semicolon
	/// </summary>
	protected const int BT_SEMI = BT_SOL + 1;
	/// <summary>
	/// Hash
	/// </summary>
	protected const int BT_NUM = BT_SEMI + 1;
	/// <summary>
	/// Left square bracket
	/// </summary>
	protected const int BT_LSQB = BT_NUM + 1;
	/// <summary>
	/// space
	/// </summary>
	protected const int BT_S = BT_LSQB + 1;
	/// <summary>
	/// 
	/// </summary>
	protected const int BT_NMSTRT = BT_S + 1;
	/// <summary>
	/// 
	/// </summary>
	protected const int BT_NAME = BT_NMSTRT + 1;
	/// <summary>
	/// Minus
	/// </summary>
	protected const int BT_MINUS = BT_NAME + 1;
	/// <summary>
	/// Other
	/// </summary>
	protected const int BT_OTHER = BT_MINUS + 1;
	/// <summary>
	/// Percent
	/// </summary>
	protected const int BT_PERCNT = BT_OTHER + 1;
	/// <summary>
	/// Left paren
	/// </summary>
	protected const int BT_LPAR = BT_PERCNT + 1;
	/// <summary>
	/// Right paren
	/// </summary>
	protected const int BT_RPAR = BT_LPAR + 1;
	/// <summary>
	/// 
	/// </summary>
	protected const int BT_AST = BT_RPAR + 1;
	/// <summary>
	/// +
	/// </summary>
	protected const int BT_PLUS = BT_AST + 1;
	/// <summary>
	/// ,
	/// </summary>
	protected const int BT_COMMA = BT_PLUS + 1;
	/// <summary>
	/// Pipe
	/// </summary>
	protected const int BT_VERBAR = BT_COMMA + 1;

	/// <summary>
	/// What syntax do each of the ASCII7 characters have?
	/// </summary>
	protected static readonly int[] asciiTypeTable = new int[]
	{
            /* 0x00 */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0x04 */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0x08 */ BT_NONXML, BT_S, BT_LF, BT_NONXML,
            /* 0x0C */ BT_NONXML, BT_CR, BT_NONXML, BT_NONXML,
            /* 0x10 */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0x14 */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0x18 */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0x1C */ BT_NONXML, BT_NONXML, BT_NONXML, BT_NONXML,
            /* 0x20 */ BT_S, BT_EXCL, BT_QUOT, BT_NUM,
            /* 0x24 */ BT_OTHER, BT_PERCNT, BT_AMP, BT_APOS,
            /* 0x28 */ BT_LPAR, BT_RPAR, BT_AST, BT_PLUS,
            /* 0x2C */ BT_COMMA, BT_MINUS, BT_NAME, BT_SOL,
            /* 0x30 */ BT_NAME, BT_NAME, BT_NAME, BT_NAME,
            /* 0x34 */ BT_NAME, BT_NAME, BT_NAME, BT_NAME,
            /* 0x38 */ BT_NAME, BT_NAME, BT_NMSTRT, BT_SEMI,
            /* 0x3C */ BT_LT, BT_EQUALS, BT_GT, BT_QUEST,
            /* 0x40 */ BT_OTHER, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x44 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x48 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x4C */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x50 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x54 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x58 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_LSQB,
            /* 0x5C */ BT_OTHER, BT_RSQB, BT_OTHER, BT_NMSTRT,
            /* 0x60 */ BT_OTHER, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x64 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x68 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x6C */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x70 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x74 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_NMSTRT,
            /* 0x78 */ BT_NMSTRT, BT_NMSTRT, BT_NMSTRT, BT_OTHER,
            /* 0x7C */ BT_VERBAR, BT_OTHER, BT_OTHER, BT_OTHER,
	};

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sourceBuf"></param>
	/// <param name="sourceStart"></param>
	/// <param name="sourceEnd"></param>
	/// <param name="targetBuf"></param>
	/// <param name="targetStart"></param>
	/// <returns></returns>
	protected abstract int Convert(byte[] sourceBuf,
								   int sourceStart, int sourceEnd,
								   char[] targetBuf, int targetStart);

	private static XmlEncoding utf8Encoding;

	private const byte UTF8_ENCODING = 0;
	private const byte UTF16_LITTLE_ENDIAN_ENCODING = 1;
	private const byte UTF16_BIG_ENDIAN_ENCODING = 2;
	private const byte INTERNAL_ENCODING = 3;
	private const byte ISO8859_1_ENCODING = 4;
	private const byte ASCII_ENCODING = 5;

	private static XmlEncoding GetEncoding(byte enc)
	{
		switch (enc)
		{
			case UTF8_ENCODING:
				if (utf8Encoding == null)
					utf8Encoding = new UTF8XmlEncoding();
				return utf8Encoding;
		}

		throw new NotSupportedException($"Encoding {enc} is not supported.");
	}

	private readonly int _minBytesPerChar;

	/// <summary>
	/// Constructor called by subclasses to set the minimum bytes per character
	/// </summary>
	/// <param name="minBPC"></param>
	protected XmlEncoding(int minBPC)
	{
		_minBytesPerChar = minBPC;
	}

	/// <summary>
	/// Get the byte type of the next byte.  There are guaranteed to be minBPC available bytes starting at off.
	/// </summary>
	/// <param name="buf"></param>
	/// <param name="off"></param>
	/// <returns></returns>
	protected abstract int GetByteType(byte[] buf, int off);

	/// <summary>
	/// Really only works for ASCII7.
	/// </summary>
	/// <param name="buf"></param>
	/// <param name="off"></param>
	/// <returns></returns>
	protected abstract char ConvertByteToAscii(byte[] buf, int off);

	/// <summary>
	/// This must only be called when c is an (XML significant)
	/// ASCII character.
	/// </summary>
	/// <param name="buf"></param>
	/// <param name="off"></param>
	/// <param name="c"></param>
	/// <returns></returns>
	protected abstract bool IsCharMatches(byte[] buf, int off, char c);

	/// <summary>
	/// Called only when byteType(buf, off) == BT_LEAD2
	/// </summary>
	/// <param name="buf"></param>
	/// <param name="off"></param>
	/// <returns></returns>
	protected virtual int GetByteType2(byte[] buf, int off)
	{
		return BT_OTHER;
	}

	/// <summary>
	/// Called only when byteType(buf, off) == BT_LEAD3
	/// </summary>
	/// <param name="buf"></param>
	/// <param name="off"></param>
	/// <returns></returns>
	protected virtual int GetByteType3(byte[] buf, int off)
	{
		return BT_OTHER;
	}

	/// <summary>
	/// Called only when byteType(buf, off) == BT_LEAD4
	/// </summary>
	/// <param name="buf"></param>
	/// <param name="off"></param>
	/// <returns></returns>
	protected virtual int GetByteType4(byte[] buf, int off)
	{
		return BT_OTHER;
	}

	protected virtual void CheckByte2(byte[] buf, int off)
	{

	}

	protected virtual void CheckByte3(byte[] buf, int off)
	{

	}

	protected virtual void CheckByte4(byte[] buf, int off)
	{

	}

	protected abstract void MovePosition(byte[] buf, int off, int end, Position pos);

	private void ThrowIfCharNotMatches(byte[] buf, int off, char c)
	{
		if (!IsCharMatches(buf, off, c))
			throw new InvalidTokenException(off);
	}

	protected virtual TOK ScanComment(byte[] buf, int off, int end, Token token)
	{
		if (off != end)
		{
			ThrowIfCharNotMatches(buf, off, '-');
			off += _minBytesPerChar;
			while (off != end)
			{
				switch (GetByteType(buf, off))
				{
					case BT_LEAD2:
						if (end - off < 2)
							throw new PartialCharException(off);
						CheckByte2(buf, off);
						off += 2;
						break;
					case BT_LEAD3:
						if (end - off < 3)
							throw new PartialCharException(off);
						CheckByte3(buf, off);
						off += 3;
						break;
					case BT_LEAD4:
						if (end - off < 4)
							throw new PartialCharException(off);
						CheckByte4(buf, off);
						off += 4;
						break;
					case BT_NONXML:
					case BT_MALFORM:
						throw new InvalidTokenException(off);
					case BT_MINUS:
						if ((off += _minBytesPerChar) == end)
							throw new PartialTokenException();
						if (IsCharMatches(buf, off, '-'))
						{
							if ((off += _minBytesPerChar) == end)
								throw new PartialTokenException();
							ThrowIfCharNotMatches(buf, off, '>');
							token.TokenEnd = off + _minBytesPerChar;
							return TOK.COMMENT;
						}
						break;
					default:
						off += _minBytesPerChar;
						break;
				}
			}
		}
		throw new PartialTokenException();
	}

	/* off points to character following "<!" */
	protected virtual TOK ScanDecl(byte[] buf, int off, int end, Token token)
	{
		if (off == end)
			throw new PartialTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_MINUS:
				return ScanComment(buf, off + _minBytesPerChar, end, token);
			case BT_LSQB:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.COND_SECT_OPEN;
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_PERCNT:
					if (off + _minBytesPerChar == end)
						throw new PartialTokenException();
					/* don't allow <!ENTITY% foo "whatever"> */
					switch (GetByteType(buf, off + _minBytesPerChar))
					{
						case BT_S:
						case BT_CR:
						case BT_LF:
						case BT_PERCNT:
							throw new InvalidTokenException(off);
					}
					/* fall through */
					goto case BT_S;
				case BT_S:
				case BT_CR:
				case BT_LF:
					token.TokenEnd = off;
					return TOK.DECL_OPEN;
				case BT_NMSTRT:
					off += _minBytesPerChar;
					break;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	protected virtual bool CheckTargetIsXmlDecl(byte[] buf, int off, int end)
	{
		bool upper = false;
		if (end - off != _minBytesPerChar * 3)
			return false;
		switch (ConvertByteToAscii(buf, off))
		{
			case 'x':
				break;
			case 'X':
				upper = true;
				break;
			default:
				return false;
		}
		off += _minBytesPerChar;
		switch (ConvertByteToAscii(buf, off))
		{
			case 'm':
				break;
			case 'M':
				upper = true;
				break;
			default:
				return false;
		}
		off += _minBytesPerChar;
		switch (ConvertByteToAscii(buf, off))
		{
			case 'l':
				break;
			case 'L':
				upper = true;
				break;
			default:
				return false;
		}
		if (upper)
			throw new InvalidTokenException(off, InvalidTokenType.XmlTarget);
		return true;
	}

	/* off points to character following "<?" */

	protected TOK ScanPi(byte[] buf, int off, int end, Token token)
	{
		int target = off;
		if (off == end)
			throw new PartialTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				if (GetByteType2(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				if (GetByteType3(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				if (GetByteType4(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 4;
				break;
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_S:
				case BT_CR:
				case BT_LF:
					bool isXml = CheckTargetIsXmlDecl(buf, target, off);
					token.NameEnd = off;
					off += _minBytesPerChar;
					while (off != end)
					{
						switch (GetByteType(buf, off))
						{
							case BT_LEAD2:
								if (end - off < 2)
									throw new PartialCharException(off);
								CheckByte2(buf, off);
								off += 2;
								break;
							case BT_LEAD3:
								if (end - off < 3)
									throw new PartialCharException(off);
								CheckByte3(buf, off);
								off += 3;
								break;
							case BT_LEAD4:
								if (end - off < 4)
									throw new PartialCharException(off);
								CheckByte4(buf, off);
								off += 4;
								break;
							case BT_NONXML:
							case BT_MALFORM:
								throw new InvalidTokenException(off);
							case BT_QUEST:
								off += _minBytesPerChar;
								if (off == end)
									throw new PartialTokenException();
								if (IsCharMatches(buf, off, '>'))
								{
									token.TokenEnd = off + _minBytesPerChar;
									if (isXml)
										return TOK.XML_DECL;
									else
										return TOK.PI;
								}
								break;
							default:
								off += _minBytesPerChar;
								break;
						}
					}
					throw new PartialTokenException();
				case BT_QUEST:
					token.NameEnd = off;
					off += _minBytesPerChar;
					if (off == end)
						throw new PartialTokenException();
					ThrowIfCharNotMatches(buf, off, '>');
					token.TokenEnd = off + _minBytesPerChar;
					return (CheckTargetIsXmlDecl(buf, target, token.NameEnd)
							? TOK.XML_DECL
							: TOK.PI);
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	/* off points to character following "<![" */
	private const string CDATA = "CDATA[";

	protected TOK ScanCdataSection(byte[] buf, int off, int end, Token token)
	{
		/* "CDATA[".length() == 6 */
		if (end - off < 6 * _minBytesPerChar)
			throw new PartialTokenException();
		for (int i = 0; i < CDATA.Length; i++, off += _minBytesPerChar)
			ThrowIfCharNotMatches(buf, off, CDATA[i]);
		token.TokenEnd = off;
		return TOK.CDATA_SECT_OPEN;
	}

	/**
         * Scans the first token of a byte subarrary that starts with the
         * content of a CDATA section.
         * Returns one of the following integers according to the type of token
         * that the subarray starts with:
         * <ul>
         * <li><code>TOK.DATA_CHARS</code></li>
         * <li><code>TOK.DATA_NEWLINE</code></li>
         * <li><code>TOK.CDATA_SECT_CLOSE</code></li>
         * </ul>
         * <p>
         * Information about the token is stored in <code>token</code>.
         * </p>
         * After <code>TOK.CDATA_SECT_CLOSE</code> is returned, the application
         * should use <code>tokenizeContent</code>.
         *
         * @exception EmptyTokenException if the subarray is empty
         * @exception PartialTokenException if the subarray contains only part of
         * a legal token
         * @exception InvalidTokenException if the subarrary does not start
         * with a legal token or part of one
         * @exception ExtensibleTokenException if the subarray encodes just a carriage
         * return ('\r')
         *
         * @see #TOK.DATA_CHARS
         * @see #TOK.DATA_NEWLINE
         * @see #TOK.CDATA_SECT_CLOSE
         * @see Token
         * @see EmptyTokenException
         * @see PartialTokenException
         * @see InvalidTokenException
         * @see ExtensibleTokenException
         * @see #tokenizeContent
         */
	public virtual TOK TokenizeCdataSection(byte[] buf, int off, int end,
									Token token)
	{
		if (_minBytesPerChar > 1)
			end = AdjustEnd(off, end);
		if (off == end)
			throw new EmptyTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_RSQB:
				off += _minBytesPerChar;
				if (off == end)
					throw new PartialTokenException();
				if (!IsCharMatches(buf, off, ']'))
					break;
				off += _minBytesPerChar;
				if (off == end)
					throw new PartialTokenException();
				if (!IsCharMatches(buf, off, '>'))
				{
					off -= _minBytesPerChar;
					break;
				}
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.CDATA_SECT_CLOSE;
			case BT_CR:
				off += _minBytesPerChar;
				if (off == end)
					throw new ExtensibleTokenException(TOK.DATA_NEWLINE);
				if (GetByteType(buf, off) == BT_LF)
					off += _minBytesPerChar;
				token.TokenEnd = off;
				return TOK.DATA_NEWLINE;
			case BT_LF:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.DATA_NEWLINE;
			case BT_NONXML:
			case BT_MALFORM:
				throw new InvalidTokenException(off);
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				CheckByte2(buf, off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				CheckByte3(buf, off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				CheckByte4(buf, off);
				off += 4;
				break;
			default:
				off += _minBytesPerChar;
				break;
		}
		token.TokenEnd = ExtendCdata(buf, off, end);
		return TOK.DATA_CHARS;
	}

	protected virtual int ExtendCdata(byte[] buf, int off, int end)
	{
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_LEAD2:
					if (end - off < 2)
						return off;
					CheckByte2(buf, off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						return off;
					CheckByte3(buf, off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						return off;
					CheckByte4(buf, off);
					off += 4;
					break;
				case BT_RSQB:
				case BT_NONXML:
				case BT_MALFORM:
				case BT_CR:
				case BT_LF:
					return off;
				default:
					off += _minBytesPerChar;
					break;
			}
		}
		return off;
	}


	/* off points to character following "</" */
	protected virtual TOK ScanEndTag(byte[] buf, int off, int end, Token token)
	{
		if (off == end)
			throw new PartialTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				if (GetByteType2(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				if (GetByteType3(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				if (GetByteType4(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 4;
				break;
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_S:
				case BT_CR:
				case BT_LF:
					token.NameEnd = off;
					for (off += _minBytesPerChar; off != end; off += _minBytesPerChar)
					{
						switch (GetByteType(buf, off))
						{
							case BT_S:
							case BT_CR:
							case BT_LF:
								break;
							case BT_GT:
								token.TokenEnd = off + _minBytesPerChar;
								return TOK.END_TAG;
							default:
								throw new InvalidTokenException(off);
						}
					}
					throw new PartialTokenException();
				case BT_GT:
					token.NameEnd = off;
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.END_TAG;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	/* off points to character following "&#X" */
	protected virtual TOK ScanHexCharRef(byte[] buf, int off, int end, Token token)
	{
		if (off != end)
		{
			int c = ConvertByteToAscii(buf, off);
			int num;
			switch (c)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num = c - '0';
					break;
				case 'A':
				case 'B':
				case 'C':
				case 'D':
				case 'E':
				case 'F':
					num = c - ('A' - 10);
					break;
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
					num = c - ('a' - 10);
					break;
				default:
					throw new InvalidTokenException(off);
			}
			for (off += _minBytesPerChar; off != end; off += _minBytesPerChar)
			{
				c = ConvertByteToAscii(buf, off);
				switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = (num << 4) + c - '0';
						break;
					case 'A':
					case 'B':
					case 'C':
					case 'D':
					case 'E':
					case 'F':
						num = (num << 4) + c - ('A' - 10);
						break;
					case 'a':
					case 'b':
					case 'c':
					case 'd':
					case 'e':
					case 'f':
						num = (num << 4) + c - ('a' - 10);
						break;
					case ';':
						token.TokenEnd = off + _minBytesPerChar;
						return SetRefChar(num, token);
					default:
						throw new InvalidTokenException(off);
				}
				if (num >= 0x110000)
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	/* off points to character following "&#" */
	protected virtual TOK ScanCharRef(byte[] buf, int off, int end, Token token)
	{
		if (off != end)
		{
			int c = ConvertByteToAscii(buf, off);
			switch (c)
			{
				case 'x':
					return ScanHexCharRef(buf, off + _minBytesPerChar, end, token);
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					break;
				default:
					throw new InvalidTokenException(off);
			}

			int num = c - '0';
			for (off += _minBytesPerChar; off != end; off += _minBytesPerChar)
			{
				c = ConvertByteToAscii(buf, off);
				switch (c)
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
						num = num * 10 + (c - '0');
						if (num < 0x110000)
							break;
						/* fall through */
						goto default;
					default:
						throw new InvalidTokenException(off);
					case ';':
						token.TokenEnd = off + _minBytesPerChar;
						return SetRefChar(num, token);
				}
			}
		}
		throw new PartialTokenException();
	}

	/* num is known to be < 0x110000; return the token code */
	protected virtual TOK SetRefChar(int num, Token token)
	{
		if (num < 0x10000)
		{
			switch (charTypeTable[num >> 8][num & 0xFF])
			{
				case BT_NONXML:
				case BT_LEAD4:
				case BT_MALFORM:
					throw new InvalidTokenException(token.TokenEnd - _minBytesPerChar);
			}
			token.RefChar1 = (char)num;
			return TOK.CHAR_REF;
		}
		else
		{
			num -= 0x10000;
			token.RefChar1 = (char)((num >> 10) + 0xD800);
			token.RefChar2 = (char)((num & ((1 << 10) - 1)) + 0xDC00);
			return TOK.CHAR_PAIR_REF;
		}
	}

	protected virtual bool IsMagicEntityRef(byte[] buf, int off, int end, Token token)
	{
		switch (ConvertByteToAscii(buf, off))
		{
			case 'a':
				if (end - off < _minBytesPerChar * 4)
					break;
				switch (ConvertByteToAscii(buf, off + _minBytesPerChar))
				{
					case 'm':
						if (IsCharMatches(buf, off + _minBytesPerChar * 2, 'p')
							&& IsCharMatches(buf, off + _minBytesPerChar * 3, ';'))
						{
							token.TokenEnd = off + _minBytesPerChar * 4;
							token.RefChar1 = '&';
							return true;
						}
						break;
					case 'p':
						if (end - off >= _minBytesPerChar * 5
							&& IsCharMatches(buf, off + _minBytesPerChar * 2, 'o')
							&& IsCharMatches(buf, off + _minBytesPerChar * 3, 's')
							&& IsCharMatches(buf, off + _minBytesPerChar * 4, ';'))
						{
							token.TokenEnd = off + _minBytesPerChar * 5;
							token.RefChar1 = '\'';
							return true;
						}
						break;
				}
				break;
			case 'l':
				if (end - off >= _minBytesPerChar * 3
					&& IsCharMatches(buf, off + _minBytesPerChar, 't')
					&& IsCharMatches(buf, off + _minBytesPerChar * 2, ';'))
				{
					token.TokenEnd = off + _minBytesPerChar * 3;
					token.RefChar1 = '<';
					return true;
				}
				break;
			case 'g':
				if (end - off >= _minBytesPerChar * 3
					&& IsCharMatches(buf, off + _minBytesPerChar, 't')
					&& IsCharMatches(buf, off + _minBytesPerChar * 2, ';'))
				{
					token.TokenEnd = off + _minBytesPerChar * 3;
					token.RefChar1 = '>';
					return true;
				}
				break;
			case 'q':
				if (end - off >= _minBytesPerChar * 5
					&& IsCharMatches(buf, off + _minBytesPerChar, 'u')
					&& IsCharMatches(buf, off + _minBytesPerChar * 2, 'o')
					&& IsCharMatches(buf, off + _minBytesPerChar * 3, 't')
					&& IsCharMatches(buf, off + _minBytesPerChar * 4, ';'))
				{
					token.TokenEnd = off + _minBytesPerChar * 5;
					token.RefChar1 = '"';
					return true;
				}
				break;
		}
		return false;
	}

	/* off points to character following "&" */
	protected virtual TOK ScanRef(byte[] buf, int off, int end, Token token)
	{
		if (off == end)
			throw new PartialTokenException();
		if (IsMagicEntityRef(buf, off, end, token))
			return TOK.MAGIC_ENTITY_REF;
		switch (GetByteType(buf, off))
		{
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				if (GetByteType2(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				if (GetByteType3(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				if (GetByteType4(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 4;
				break;
			case BT_NUM:
				return ScanCharRef(buf, off + _minBytesPerChar, end, token);
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_SEMI:
					token.NameEnd = off;
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.ENTITY_REF;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	/* off points to character following first character of
           attribute name */
	protected virtual TOK ScanAtts(int nameStart, byte[] buf, int off, int end,
						 ContentToken token)
	{
		int NameEnd = -1;
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_S:
				case BT_CR:
				case BT_LF:
					NameEnd = off;
					for (; ; )
					{
						off += _minBytesPerChar;
						if (off == end)
							throw new PartialTokenException();
						switch (GetByteType(buf, off))
						{
							case BT_EQUALS:
								goto loop;
							case BT_S:
							case BT_LF:
							case BT_CR:
								break;
							default:
								throw new InvalidTokenException(off);
						}
					}
				loop:;
					/* fall through */
					goto case BT_EQUALS;
				case BT_EQUALS:
					{
						if (NameEnd < 0)
							NameEnd = off;
						int open;
						for (; ; )
						{

							off += _minBytesPerChar;
							if (off == end)
								throw new PartialTokenException();
							open = GetByteType(buf, off);
							if (open == BT_QUOT || open == BT_APOS)
								break;
							switch (open)
							{
								case BT_S:
								case BT_LF:
								case BT_CR:
									break;
								default:
									throw new InvalidTokenException(off);
							}
						}
						off += _minBytesPerChar;
						int valueStart = off;
						bool normalized = true;
						int t;
						/* in attribute value */
						for (; ; )
						{
							if (off == end)
								throw new PartialTokenException();
							t = GetByteType(buf, off);
							if (t == open)
								break;
							switch (t)
							{
								case BT_NONXML:
								case BT_MALFORM:
									throw new InvalidTokenException(off);
								case BT_LEAD2:
									if (end - off < 2)
										throw new PartialCharException(off);
									CheckByte2(buf, off);
									off += 2;
									break;
								case BT_LEAD3:
									if (end - off < 3)
										throw new PartialCharException(off);
									CheckByte3(buf, off);
									off += 3;
									break;
								case BT_LEAD4:
									if (end - off < 4)
										throw new PartialCharException(off);
									CheckByte4(buf, off);
									off += 4;
									break;
								case BT_AMP:
									{
										normalized = false;
										int saveNameEnd = token.NameEnd;
										ScanRef(buf, off + _minBytesPerChar, end, token);
										token.NameEnd = saveNameEnd;
										off = token.TokenEnd;
										break;
									}
								case BT_S:
									if (normalized
										&& (off == valueStart
											|| ConvertByteToAscii(buf, off) != ' '
											|| (off + _minBytesPerChar != end
												&& (ConvertByteToAscii(buf, off + _minBytesPerChar) == ' '
													|| GetByteType(buf, off + _minBytesPerChar) == open))))
										normalized = false;
									off += _minBytesPerChar;
									break;
								case BT_LT:
									throw new InvalidTokenException(off);
								case BT_LF:
								case BT_CR:
									normalized = false;
									/* fall through */
									goto default;
								default:
									off += _minBytesPerChar;
									break;
							}
						}
						token.AppendAttribute(nameStart, NameEnd, valueStart,
											  off,
											  normalized);
						off += _minBytesPerChar;
						if (off == end)
							throw new PartialTokenException();
						t = GetByteType(buf, off);
						switch (t)
						{
							case BT_S:
							case BT_CR:
							case BT_LF:
								off += _minBytesPerChar;
								if (off == end)
									throw new PartialTokenException();
								t = GetByteType(buf, off);
								break;
							case BT_GT:
							case BT_SOL:
								break;
							default:
								throw new InvalidTokenException(off);
						}
						/* off points to closing quote */
						for (; ; )
						{
							switch (t)
							{
								case BT_NMSTRT:
									nameStart = off;
									off += _minBytesPerChar;
									goto skipToName;
								case BT_LEAD2:
									if (end - off < 2)
										throw new PartialCharException(off);
									if (GetByteType2(buf, off) != BT_NMSTRT)
										throw new InvalidTokenException(off);
									nameStart = off;
									off += 2;
									goto skipToName;
								case BT_LEAD3:
									if (end - off < 3)
										throw new PartialCharException(off);
									if (GetByteType3(buf, off) != BT_NMSTRT)
										throw new InvalidTokenException(off);
									nameStart = off;
									off += 3;
									goto skipToName;
								case BT_LEAD4:
									if (end - off < 4)
										throw new PartialCharException(off);
									if (GetByteType4(buf, off) != BT_NMSTRT)
										throw new InvalidTokenException(off);
									nameStart = off;
									off += 4;
									goto skipToName;
								case BT_S:
								case BT_CR:
								case BT_LF:
									break;
								case BT_GT:
									token.CheckAttributeUniqueness(buf);
									token.TokenEnd = off + _minBytesPerChar;
									return TOK.START_TAG_WITH_ATTS;
								case BT_SOL:
									off += _minBytesPerChar;
									if (off == end)
										throw new PartialTokenException();
									ThrowIfCharNotMatches(buf, off, '>');
									token.CheckAttributeUniqueness(buf);
									token.TokenEnd = off + _minBytesPerChar;
									return TOK.EMPTY_ELEMENT_WITH_ATTS;
								default:
									throw new InvalidTokenException(off);
							}
							off += _minBytesPerChar;
							if (off == end)
								throw new PartialTokenException();
							t = GetByteType(buf, off);
						}

					skipToName:
						NameEnd = -1;
						break;
					}
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	/* off points to character following "<" */
	protected virtual TOK ScanLt(byte[] buf, int off, int end, ContentToken token)
	{
		if (off == end)
			throw new PartialTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				if (GetByteType2(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				if (GetByteType3(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				if (GetByteType4(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 4;
				break;
			case BT_EXCL:
				if ((off += _minBytesPerChar) == end)
					throw new PartialTokenException();
				switch (GetByteType(buf, off))
				{
					case BT_MINUS:
						return ScanComment(buf, off + _minBytesPerChar, end, token);
					case BT_LSQB:
						return ScanCdataSection(buf, off + _minBytesPerChar, end, token);
				}
				throw new InvalidTokenException(off);
			case BT_QUEST:
				return ScanPi(buf, off + _minBytesPerChar, end, token);
			case BT_SOL:
				return ScanEndTag(buf, off + _minBytesPerChar, end, token);
			default:
				throw new InvalidTokenException(off);
		}
		/* we have a start-tag */
		token.NameEnd = -1;
		token.Clear();

		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_S:
				case BT_CR:
				case BT_LF:
					token.NameEnd = off;
					off += _minBytesPerChar;
					for (; ; )
					{
						if (off == end)
							throw new PartialTokenException();
						switch (GetByteType(buf, off))
						{
							case BT_NMSTRT:
								return ScanAtts(off, buf, off + _minBytesPerChar, end, token);
							case BT_LEAD2:
								if (end - off < 2)
									throw new PartialCharException(off);
								if (GetByteType2(buf, off) != BT_NMSTRT)
									throw new InvalidTokenException(off);
								return ScanAtts(off, buf, off + 2, end, token);
							case BT_LEAD3:
								if (end - off < 3)
									throw new PartialCharException(off);
								if (GetByteType3(buf, off) != BT_NMSTRT)
									throw new InvalidTokenException(off);
								return ScanAtts(off, buf, off + 3, end, token);
							case BT_LEAD4:
								if (end - off < 4)
									throw new PartialCharException(off);
								if (GetByteType4(buf, off) != BT_NMSTRT)
									throw new InvalidTokenException(off);
								return ScanAtts(off, buf, off + 4, end, token);
							case BT_GT:
							case BT_SOL:
								goto loop;
							case BT_S:
							case BT_CR:
							case BT_LF:
								off += _minBytesPerChar;
								break;
							default:
								throw new InvalidTokenException(off);
						}
					}
				loop:
					break;
				case BT_GT:
					if (token.NameEnd < 0)
						token.NameEnd = off;
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.START_TAG_NO_ATTS;
				case BT_SOL:
					if (token.NameEnd < 0)
						token.NameEnd = off;
					off += _minBytesPerChar;
					if (off == end)
						throw new PartialTokenException();
					ThrowIfCharNotMatches(buf, off, '>');
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.EMPTY_ELEMENT_NO_ATTS;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}

	protected virtual int AdjustEnd(int off, int end)
	{
		int n = end - off;
		if ((n & (_minBytesPerChar - 1)) != 0)
		{
			n &= ~(_minBytesPerChar - 1);
			if (n == 0)
				throw new PartialCharException(off);
			return off + n;
		}
		else
			return end;
	}

	public virtual TOK TokenizeContent(byte[] buf, int off, int end,
							   ContentToken token)
	{
		if (_minBytesPerChar > 1)
			end = AdjustEnd(off, end);
		if (off == end)
			throw new EmptyTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_LT:
				return ScanLt(buf, off + _minBytesPerChar, end, token);
			case BT_AMP:
				return ScanRef(buf, off + _minBytesPerChar, end, token);
			case BT_CR:
				off += _minBytesPerChar;
				if (off == end)
					throw new ExtensibleTokenException(TOK.DATA_NEWLINE);
				if (GetByteType(buf, off) == BT_LF)
					off += _minBytesPerChar;
				token.TokenEnd = off;
				return TOK.DATA_NEWLINE;
			case BT_LF:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.DATA_NEWLINE;
			case BT_RSQB:
				off += _minBytesPerChar;
				if (off == end)
					throw new ExtensibleTokenException(TOK.DATA_CHARS);
				if (!IsCharMatches(buf, off, ']'))
					break;
				off += _minBytesPerChar;
				if (off == end)
					throw new ExtensibleTokenException(TOK.DATA_CHARS);
				if (!IsCharMatches(buf, off, '>'))
				{
					off -= _minBytesPerChar;
					break;
				}
				throw new InvalidTokenException(off);
			case BT_NONXML:
			case BT_MALFORM:
				throw new InvalidTokenException(off);
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				CheckByte2(buf, off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				CheckByte3(buf, off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				CheckByte4(buf, off);
				off += 4;
				break;
			default:
				off += _minBytesPerChar;
				break;
		}
		token.TokenEnd = ExtendData(buf, off, end);
		return TOK.DATA_CHARS;
	}

	protected virtual int ExtendData(byte[] buf, int off, int end)
	{
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_LEAD2:
					if (end - off < 2)
						return off;
					CheckByte2(buf, off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						return off;
					CheckByte3(buf, off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						return off;
					CheckByte4(buf, off);
					off += 4;
					break;
				case BT_RSQB:
				case BT_AMP:
				case BT_LT:
				case BT_NONXML:
				case BT_MALFORM:
				case BT_CR:
				case BT_LF:
					return off;
				default:
					off += _minBytesPerChar;
					break;
			}
		}
		return off;
	}

	/* off points to character following "%" */
	protected virtual TOK ScanPercent(byte[] buf, int off, int end, Token token)
	{
		if (off == end)
			throw new PartialTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				if (GetByteType2(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				if (GetByteType3(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				if (GetByteType4(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 4;
				break;
			case BT_S:
			case BT_LF:
			case BT_CR:
			case BT_PERCNT:
				token.TokenEnd = off;
				return TOK.PERCENT;
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_SEMI:
					token.NameEnd = off;
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.PARAM_ENTITY_REF;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new PartialTokenException();
	}


	protected virtual TOK ScanPoundName(byte[] buf, int off, int end, Token token)
	{
		if (off == end)
			throw new PartialTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_NMSTRT:
				off += _minBytesPerChar;
				break;
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				if (GetByteType2(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 2;
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				if (GetByteType3(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 3;
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				if (GetByteType4(buf, off) != BT_NMSTRT)
					throw new InvalidTokenException(off);
				off += 4;
				break;
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_CR:
				case BT_LF:
				case BT_S:
				case BT_RPAR:
				case BT_GT:
				case BT_PERCNT:
				case BT_VERBAR:
					token.TokenEnd = off;
					return TOK.POUND_NAME;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new ExtensibleTokenException(TOK.POUND_NAME);
	}

	protected virtual TOK ScanLit(int open, byte[] buf, int off, int end, Token token)
	{
		while (off != end)
		{
			int t = GetByteType(buf, off);
			switch (t)
			{
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialTokenException();
					CheckByte2(buf, off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialTokenException();
					CheckByte3(buf, off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialTokenException();
					CheckByte4(buf, off);
					off += 4;
					break;
				case BT_NONXML:
				case BT_MALFORM:
					throw new InvalidTokenException(off);
				case BT_QUOT:
				case BT_APOS:
					off += _minBytesPerChar;
					if (t != open)
						break;
					if (off == end)
						throw new ExtensibleTokenException(TOK.LITERAL);
					switch (GetByteType(buf, off))
					{
						case BT_S:
						case BT_CR:
						case BT_LF:
						case BT_GT:
						case BT_PERCNT:
						case BT_LSQB:
							token.TokenEnd = off;
							return TOK.LITERAL;
						default:
							throw new InvalidTokenException(off);
					}
				default:
					off += _minBytesPerChar;
					break;
			}
		}
		throw new PartialTokenException();
	}

	public static XmlEncoding GetInitialEncoding(byte[] buf, int off, int end, Token token)
	{
		token.TokenEnd = off;

		switch (end - off)
		{
			case 0:
				break;
			case 1:
				if (buf[off] > 127)
					return null;
				break;
			default:
				int b0 = buf[off] & 0xFF;
				int b1 = buf[off + 1] & 0xFF;
				switch ((b0 << 8) | b1)
				{
					case 0xFEFF:
						token.TokenEnd = off + 2;
						/* fall through */
						goto case '<';
					case '<': /* not legal; but not a fatal error */
						return GetEncoding(UTF16_BIG_ENDIAN_ENCODING);
					case 0xFFFE:
						token.TokenEnd = off + 2;
						/* fall through */
						goto case '<' << 8;
					case '<' << 8:  /* not legal; but not a fatal error */
						return GetEncoding(UTF16_LITTLE_ENDIAN_ENCODING);
				}
				break;
		}
		return GetEncoding(UTF8_ENCODING);
	}

	public XmlEncoding GetEncoding(string name)
	{
		if (string.IsNullOrEmpty(name))
			return this;

		switch (name.ToUpper())
		{
			case "UTF-8":
				return GetEncoding(UTF8_ENCODING);
		}
		return null;
	}

	public virtual XmlEncoding GetSingleByteEncoding(string map)
		=> throw new NotImplementedException();

	public static XmlEncoding GetInternalEncoding()
		=> GetEncoding(INTERNAL_ENCODING);

	public virtual TOK TokenizeProlog(byte[] buf, int off, int end, Token token)
	{
		TOK tok;
		if (_minBytesPerChar > 1)
			end = AdjustEnd(off, end);
		if (off == end)
			throw new EmptyTokenException();
		switch (GetByteType(buf, off))
		{
			case BT_QUOT:
				return ScanLit(BT_QUOT, buf, off + _minBytesPerChar, end, token);
			case BT_APOS:
				return ScanLit(BT_APOS, buf, off + _minBytesPerChar, end, token);
			case BT_LT:
				{
					off += _minBytesPerChar;
					if (off == end)
						throw new PartialTokenException();
					switch (GetByteType(buf, off))
					{
						case BT_EXCL:
							return ScanDecl(buf, off + _minBytesPerChar, end, token);
						case BT_QUEST:
							return ScanPi(buf, off + _minBytesPerChar, end, token);
						case BT_NMSTRT:
						case BT_LEAD2:
						case BT_LEAD3:
						case BT_LEAD4:
							token.TokenEnd = off - _minBytesPerChar;
							throw new EndOfPrologException();
					}
					throw new InvalidTokenException(off);
				}
			case BT_CR:
				if (off + _minBytesPerChar == end)
					throw new ExtensibleTokenException(TOK.PROLOG_S);
				/* fall through */
				goto case BT_S;
			case BT_S:
			case BT_LF:
				for (; ; )
				{
					off += _minBytesPerChar;
					if (off == end)
						break;
					switch (GetByteType(buf, off))
					{
						case BT_S:
						case BT_LF:
							break;
						case BT_CR:
							/* don't split CR/LF pair */
							if (off + _minBytesPerChar != end)
								break;
							/* fall through */
							goto default;
						default:
							token.TokenEnd = off;
							return TOK.PROLOG_S;
					}
				}
				token.TokenEnd = off;
				return TOK.PROLOG_S;
			case BT_PERCNT:
				return ScanPercent(buf, off + _minBytesPerChar, end, token);
			case BT_COMMA:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.COMMA;
			case BT_LSQB:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.OPEN_BRACKET;
			case BT_RSQB:
				off += _minBytesPerChar;
				if (off == end)
					throw new ExtensibleTokenException(TOK.CLOSE_BRACKET);
				if (IsCharMatches(buf, off, ']'))
				{
					if (off + _minBytesPerChar == end)
						throw new PartialTokenException();
					if (IsCharMatches(buf, off + _minBytesPerChar, '>'))
					{
						token.TokenEnd = off + 2 * _minBytesPerChar;
						return TOK.COND_SECT_CLOSE;
					}
				}
				token.TokenEnd = off;
				return TOK.CLOSE_BRACKET;
			case BT_LPAR:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.OPEN_PAREN;
			case BT_RPAR:
				off += _minBytesPerChar;
				if (off == end)
					throw new ExtensibleTokenException(TOK.CLOSE_PAREN);
				switch (GetByteType(buf, off))
				{
					case BT_AST:
						token.TokenEnd = off + _minBytesPerChar;
						return TOK.CLOSE_PAREN_ASTERISK;
					case BT_QUEST:
						token.TokenEnd = off + _minBytesPerChar;
						return TOK.CLOSE_PAREN_QUESTION;
					case BT_PLUS:
						token.TokenEnd = off + _minBytesPerChar;
						return TOK.CLOSE_PAREN_PLUS;
					case BT_CR:
					case BT_LF:
					case BT_S:
					case BT_GT:
					case BT_COMMA:
					case BT_VERBAR:
					case BT_RPAR:
						token.TokenEnd = off;
						return TOK.CLOSE_PAREN;
				}
				throw new InvalidTokenException(off);
			case BT_VERBAR:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.OR;
			case BT_GT:
				token.TokenEnd = off + _minBytesPerChar;
				return TOK.DECL_CLOSE;
			case BT_NUM:
				return ScanPoundName(buf, off + _minBytesPerChar, end, token);
			case BT_LEAD2:
				if (end - off < 2)
					throw new PartialCharException(off);
				switch (GetByteType2(buf, off))
				{
					case BT_NMSTRT:
						off += 2;
						tok = TOK.NAME;
						break;
					case BT_NAME:
						off += 2;
						tok = TOK.NMTOKEN;
						break;
					default:
						throw new InvalidTokenException(off);
				}
				break;
			case BT_LEAD3:
				if (end - off < 3)
					throw new PartialCharException(off);
				switch (GetByteType3(buf, off))
				{
					case BT_NMSTRT:
						off += 3;
						tok = TOK.NAME;
						break;
					case BT_NAME:
						off += 3;
						tok = TOK.NMTOKEN;
						break;
					default:
						throw new InvalidTokenException(off);
				}
				break;
			case BT_LEAD4:
				if (end - off < 4)
					throw new PartialCharException(off);
				switch (GetByteType4(buf, off))
				{
					case BT_NMSTRT:
						off += 4;
						tok = TOK.NAME;
						break;
					case BT_NAME:
						off += 4;
						tok = TOK.NMTOKEN;
						break;
					default:
						throw new InvalidTokenException(off);
				}
				break;
			case BT_NMSTRT:
				tok = TOK.NAME;
				off += _minBytesPerChar;
				break;
			case BT_NAME:
			case BT_MINUS:
				tok = TOK.NMTOKEN;
				off += _minBytesPerChar;
				break;
			default:
				throw new InvalidTokenException(off);
		}
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_NMSTRT:
				case BT_NAME:
				case BT_MINUS:
					off += _minBytesPerChar;
					break;
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					if (!IsNameChar2(buf, off))
						throw new InvalidTokenException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					if (!IsNameChar3(buf, off))
						throw new InvalidTokenException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					if (!IsNameChar4(buf, off))
						throw new InvalidTokenException(off);
					off += 4;
					break;
				case BT_GT:
				case BT_RPAR:
				case BT_COMMA:
				case BT_VERBAR:
				case BT_LSQB:
				case BT_PERCNT:
				case BT_S:
				case BT_CR:
				case BT_LF:
					token.TokenEnd = off;
					return tok;
				case BT_PLUS:
					if (tok != TOK.NAME)
						throw new InvalidTokenException(off);
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.NAME_PLUS;
				case BT_AST:
					if (tok != TOK.NAME)
						throw new InvalidTokenException(off);
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.NAME_ASTERISK;
				case BT_QUEST:
					if (tok != TOK.NAME)
						throw new InvalidTokenException(off);
					token.TokenEnd = off + _minBytesPerChar;
					return TOK.NAME_QUESTION;
				default:
					throw new InvalidTokenException(off);
			}
		}
		throw new ExtensibleTokenException(tok);
	}

	public virtual TOK TokenizeAttributeValue(byte[] buf, int off, int end, Token token)
	{
		if (_minBytesPerChar > 1)
			end = AdjustEnd(off, end);
		if (off == end)
			throw new EmptyTokenException();
		int start = off;
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					off += 4;
					break;
				case BT_AMP:
					if (off == start)
						return ScanRef(buf, off + _minBytesPerChar, end, token);
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				case BT_LT:
					/* this is for inside entity references */
					throw new InvalidTokenException(off);
				case BT_S:
					if (off == start)
					{
						token.TokenEnd = off + _minBytesPerChar;
						return TOK.ATTRIBUTE_VALUE_S;
					}
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				case BT_LF:
					if (off == start)
					{
						token.TokenEnd = off + _minBytesPerChar;
						return TOK.DATA_NEWLINE;
					}
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				case BT_CR:
					if (off == start)
					{
						off += _minBytesPerChar;
						if (off == end)
							throw new ExtensibleTokenException(TOK.DATA_NEWLINE);
						if (GetByteType(buf, off) == BT_LF)
							off += _minBytesPerChar;
						token.TokenEnd = off;
						return TOK.DATA_NEWLINE;
					}
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				default:
					off += _minBytesPerChar;
					break;
			}
		}
		token.TokenEnd = off;
		return TOK.DATA_CHARS;
	}

	public virtual TOK TokenizeEntityValue(byte[] buf, int off, int end,
								   Token token)
	{
		if (_minBytesPerChar > 1)
			end = AdjustEnd(off, end);
		if (off == end)
			throw new EmptyTokenException();
		int start = off;
		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					off += 4;
					break;
				case BT_AMP:
					if (off == start)
						return ScanRef(buf, off + _minBytesPerChar, end, token);
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				case BT_PERCNT:
					if (off == start)
						return ScanPercent(buf, off + _minBytesPerChar, end, token);
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				case BT_LF:
					if (off == start)
					{
						token.TokenEnd = off + _minBytesPerChar;
						return TOK.DATA_NEWLINE;
					}
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				case BT_CR:
					if (off == start)
					{
						off += _minBytesPerChar;
						if (off == end)
							throw new ExtensibleTokenException(TOK.DATA_NEWLINE);
						if (GetByteType(buf, off) == BT_LF)
							off += _minBytesPerChar;
						token.TokenEnd = off;
						return TOK.DATA_NEWLINE;
					}
					token.TokenEnd = off;
					return TOK.DATA_CHARS;
				default:
					off += _minBytesPerChar;
					break;
			}
		}
		token.TokenEnd = off;
		return TOK.DATA_CHARS;
	}

	public virtual int SkipIgnoreSect(byte[] buf, int off, int end)
	{
		if (_minBytesPerChar > 1)
			end = AdjustEnd(off, end);

		int level = 0;

		while (off != end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_LEAD2:
					if (end - off < 2)
						throw new PartialCharException(off);
					CheckByte2(buf, off);
					off += 2;
					break;
				case BT_LEAD3:
					if (end - off < 3)
						throw new PartialCharException(off);
					CheckByte3(buf, off);
					off += 3;
					break;
				case BT_LEAD4:
					if (end - off < 4)
						throw new PartialCharException(off);
					CheckByte4(buf, off);
					off += 4;
					break;
				case BT_NONXML:
				case BT_MALFORM:
					throw new InvalidTokenException(off);
				case BT_LT:
					off += _minBytesPerChar;
					if (off == end)
						goto loop;
					if (!IsCharMatches(buf, off, '!'))
						break;
					off += _minBytesPerChar;
					if (off == end)
						goto loop;
					if (!IsCharMatches(buf, off, '['))
						break;
					level++;
					off += _minBytesPerChar;
					break;
				case BT_RSQB:
					off += _minBytesPerChar;
					if (off == end)
						goto loop;
					if (!IsCharMatches(buf, off, ']'))
						break;
					off += _minBytesPerChar;
					if (off == end)
						goto loop;
					if (IsCharMatches(buf, off, '>'))
					{
						if (level == 0)
							return off + _minBytesPerChar;
						level--;
					}
					else if (IsCharMatches(buf, off, ']'))
						break;
					off += _minBytesPerChar;
					break;
				default:
					off += _minBytesPerChar;
					break;
			}
		}
	loop:
		throw new PartialTokenException();
	}

	public virtual string GetPublicId(byte[] buf, int off, int end)
	{
		System.Text.StringBuilder sbuf = new System.Text.StringBuilder();
		off += _minBytesPerChar;
		end -= _minBytesPerChar;
		for (; off != end; off += _minBytesPerChar)
		{
			char c = (char)ConvertByteToAscii(buf, off);
			switch (GetByteType(buf, off))
			{
				case BT_MINUS:
				case BT_APOS:
				case BT_LPAR:
				case BT_RPAR:
				case BT_PLUS:
				case BT_COMMA:
				case BT_SOL:
				case BT_EQUALS:
				case BT_QUEST:
				case BT_SEMI:
				case BT_EXCL:
				case BT_AST:
				case BT_PERCNT:
				case BT_NUM:
					sbuf.Append(c);
					break;
				case BT_S:
					if (IsCharMatches(buf, off, '\t'))
						throw new InvalidTokenException(off);
					/* fall through */
					goto case BT_CR;
				case BT_CR:
				case BT_LF:
					if ((sbuf.Length > 0) && (sbuf[sbuf.Length - 1] != ' '))
						sbuf.Append(' ');
					break;
				case BT_NAME:
				case BT_NMSTRT:
					if ((c & ~0x7f) == 0)
					{
						sbuf.Append(c);
						break;
					}
					// fall through
					goto default;
				default:
					switch (c)
					{
						case '$':
						case '@':
							break;
						default:
							throw new InvalidTokenException(off);
					}
					break;
			}
		}
		if (sbuf.Length > 0 && sbuf[sbuf.Length - 1] == ' ')
			sbuf.Length = sbuf.Length - 1;
		return sbuf.ToString();
	}

	public virtual bool MatchesXmlStringEquality(byte[] buf, int off, int end, string str)
	{
		int len = str.Length;

		if (len * _minBytesPerChar != end - off)
			return false;

		for (int i = 0; i < len; off += _minBytesPerChar, i++)
		{
			if (!IsCharMatches(buf, off, str[i]))
				return false;
		}

		return true;
	}

	public virtual int SkipWhitespaces(byte[] buf, int off, int end)
	{
		while (off < end)
		{
			switch (GetByteType(buf, off))
			{
				case BT_S:
				case BT_CR:
				case BT_LF:
					off += _minBytesPerChar;
					break;
				default:
					goto loop;
			}
		}
	loop:
		return off;
	}

	protected virtual bool IsNameChar2(byte[] buf, int off)
	{
		int bt = GetByteType2(buf, off);
		return bt == BT_NAME || bt == BT_NMSTRT;
	}

	protected virtual bool IsNameChar3(byte[] buf, int off)
	{
		int bt = GetByteType3(buf, off);
		return bt == BT_NAME || bt == BT_NMSTRT;
	}

	protected virtual bool IsNameChar4(byte[] buf, int off)
	{
		int bt = GetByteType4(buf, off);
		return bt == BT_NAME || bt == BT_NMSTRT;
	}

	private const string nameStartSingles =
  "\u003a\u005f\u0386\u038c\u03da\u03dc\u03de\u03e0\u0559\u06d5\u093d\u09b2" +
  "\u0a5e\u0a8d\u0abd\u0ae0\u0b3d\u0b9c\u0cde\u0e30\u0e84\u0e8a\u0e8d\u0ea5" +
  "\u0ea7\u0eb0\u0ebd\u1100\u1109\u113c\u113e\u1140\u114c\u114e\u1150\u1159" +
  "\u1163\u1165\u1167\u1169\u1175\u119e\u11a8\u11ab\u11ba\u11eb\u11f0\u11f9" +
  "\u1f59\u1f5b\u1f5d\u1fbe\u2126\u212e\u3007";

	private const string nameStartRanges =
  "\u0041\u005a\u0061\u007a\u00c0\u00d6\u00d8\u00f6\u00f8\u00ff\u0100\u0131" +
  "\u0134\u013e\u0141\u0148\u014a\u017e\u0180\u01c3\u01cd\u01f0\u01f4\u01f5" +
  "\u01fa\u0217\u0250\u02a8\u02bb\u02c1\u0388\u038a\u038e\u03a1\u03a3\u03ce" +
  "\u03d0\u03d6\u03e2\u03f3\u0401\u040c\u040e\u044f\u0451\u045c\u045e\u0481" +
  "\u0490\u04c4\u04c7\u04c8\u04cb\u04cc\u04d0\u04eb\u04ee\u04f5\u04f8\u04f9" +
  "\u0531\u0556\u0561\u0586\u05d0\u05ea\u05f0\u05f2\u0621\u063a\u0641\u064a" +
  "\u0671\u06b7\u06ba\u06be\u06c0\u06ce\u06d0\u06d3\u06e5\u06e6\u0905\u0939" +
  "\u0958\u0961\u0985\u098c\u098f\u0990\u0993\u09a8\u09aa\u09b0\u09b6\u09b9" +
  "\u09dc\u09dd\u09df\u09e1\u09f0\u09f1\u0a05\u0a0a\u0a0f\u0a10\u0a13\u0a28" +
  "\u0a2a\u0a30\u0a32\u0a33\u0a35\u0a36\u0a38\u0a39\u0a59\u0a5c\u0a72\u0a74" +
  "\u0a85\u0a8b\u0a8f\u0a91\u0a93\u0aa8\u0aaa\u0ab0\u0ab2\u0ab3\u0ab5\u0ab9" +
  "\u0b05\u0b0c\u0b0f\u0b10\u0b13\u0b28\u0b2a\u0b30\u0b32\u0b33\u0b36\u0b39" +
  "\u0b5c\u0b5d\u0b5f\u0b61\u0b85\u0b8a\u0b8e\u0b90\u0b92\u0b95\u0b99\u0b9a" +
  "\u0b9e\u0b9f\u0ba3\u0ba4\u0ba8\u0baa\u0bae\u0bb5\u0bb7\u0bb9\u0c05\u0c0c" +
  "\u0c0e\u0c10\u0c12\u0c28\u0c2a\u0c33\u0c35\u0c39\u0c60\u0c61\u0c85\u0c8c" +
  "\u0c8e\u0c90\u0c92\u0ca8\u0caa\u0cb3\u0cb5\u0cb9\u0ce0\u0ce1\u0d05\u0d0c" +
  "\u0d0e\u0d10\u0d12\u0d28\u0d2a\u0d39\u0d60\u0d61\u0e01\u0e2e\u0e32\u0e33" +
  "\u0e40\u0e45\u0e81\u0e82\u0e87\u0e88\u0e94\u0e97\u0e99\u0e9f\u0ea1\u0ea3" +
  "\u0eaa\u0eab\u0ead\u0eae\u0eb2\u0eb3\u0ec0\u0ec4\u0f40\u0f47\u0f49\u0f69" +
  "\u10a0\u10c5\u10d0\u10f6\u1102\u1103\u1105\u1107\u110b\u110c\u110e\u1112" +
  "\u1154\u1155\u115f\u1161\u116d\u116e\u1172\u1173\u11ae\u11af\u11b7\u11b8" +
  "\u11bc\u11c2\u1e00\u1e9b\u1ea0\u1ef9\u1f00\u1f15\u1f18\u1f1d\u1f20\u1f45" +
  "\u1f48\u1f4d\u1f50\u1f57\u1f5f\u1f7d\u1f80\u1fb4\u1fb6\u1fbc\u1fc2\u1fc4" +
  "\u1fc6\u1fcc\u1fd0\u1fd3\u1fd6\u1fdb\u1fe0\u1fec\u1ff2\u1ff4\u1ff6\u1ffc" +
  "\u212a\u212b\u2180\u2182\u3041\u3094\u30a1\u30fa\u3105\u312c\uac00\ud7a3" +
  "\u4e00\u9fa5\u3021\u3029";

	private const string nameSingles =
  "\u002d\u002e\u05bf\u05c4\u0670\u093c\u094d\u09bc\u09be\u09bf\u09d7\u0a02" +
  "\u0a3c\u0a3e\u0a3f\u0abc\u0b3c\u0bd7\u0d57\u0e31\u0eb1\u0f35\u0f37\u0f39" +
  "\u0f3e\u0f3f\u0f97\u0fb9\u20e1\u3099\u309a\u00b7\u02d0\u02d1\u0387\u0640" +
  "\u0e46\u0ec6\u3005";

	private const string nameRanges =
  "\u0300\u0345\u0360\u0361\u0483\u0486\u0591\u05a1\u05a3\u05b9\u05bb\u05bd" +
  "\u05c1\u05c2\u064b\u0652\u06d6\u06dc\u06dd\u06df\u06e0\u06e4\u06e7\u06e8" +
  "\u06ea\u06ed\u0901\u0903\u093e\u094c\u0951\u0954\u0962\u0963\u0981\u0983" +
  "\u09c0\u09c4\u09c7\u09c8\u09cb\u09cd\u09e2\u09e3\u0a40\u0a42\u0a47\u0a48" +
  "\u0a4b\u0a4d\u0a70\u0a71\u0a81\u0a83\u0abe\u0ac5\u0ac7\u0ac9\u0acb\u0acd" +
  "\u0b01\u0b03\u0b3e\u0b43\u0b47\u0b48\u0b4b\u0b4d\u0b56\u0b57\u0b82\u0b83" +
  "\u0bbe\u0bc2\u0bc6\u0bc8\u0bca\u0bcd\u0c01\u0c03\u0c3e\u0c44\u0c46\u0c48" +
  "\u0c4a\u0c4d\u0c55\u0c56\u0c82\u0c83\u0cbe\u0cc4\u0cc6\u0cc8\u0cca\u0ccd" +
  "\u0cd5\u0cd6\u0d02\u0d03\u0d3e\u0d43\u0d46\u0d48\u0d4a\u0d4d\u0e34\u0e3a" +
  "\u0e47\u0e4e\u0eb4\u0eb9\u0ebb\u0ebc\u0ec8\u0ecd\u0f18\u0f19\u0f71\u0f84" +
  "\u0f86\u0f8b\u0f90\u0f95\u0f99\u0fad\u0fb1\u0fb7\u20d0\u20dc\u302a\u302f" +
  "\u0030\u0039\u0660\u0669\u06f0\u06f9\u0966\u096f\u09e6\u09ef\u0a66\u0a6f" +
  "\u0ae6\u0aef\u0b66\u0b6f\u0be7\u0bef\u0c66\u0c6f\u0ce6\u0cef\u0d66\u0d6f" +
  "\u0e50\u0e59\u0ed0\u0ed9\u0f20\u0f29\u3031\u3035\u309d\u309e\u30fc\u30fe";

	/// <summary>
	/// 
	/// </summary>
	protected static int[][] charTypeTable;
	private static void setCharType(char c, int type)
	{
		if (c < 0x80)
			return;
		int hi = c >> 8;
		if (charTypeTable[hi] == null)
		{
			charTypeTable[hi] = new int[256];
			for (int i = 0; i < 256; i++)
				charTypeTable[hi][i] = BT_OTHER;
		}
		charTypeTable[hi][c & 0xFF] = type;
	}

	private static void setCharType(char min, char max, int type)
	{
		int[] shared = null;
		do
		{
			if ((min & 0xFF) == 0)
			{
				for (; min + (char)0xFF <= max; min += (char)0x100)
				{
					if (shared == null)
					{
						shared = new int[256];
						for (int i = 0; i < 256; i++)
							shared[i] = type;
					}
					charTypeTable[min >> 8] = shared;
					if (min + 0xFF == max)
						return;
				}
			}
			setCharType(min, type);
		} while (min++ != max);
	}

	static XmlEncoding()
	{
		charTypeTable = new int[256][];
		foreach (char c in nameSingles)
			setCharType(c, BT_NAME);
		for (int i = 0; i < nameRanges.Length; i += 2)
			setCharType(nameRanges[i], nameRanges[i + 1], BT_NAME);
		for (int i = 0; i < nameStartSingles.Length; i++)
			setCharType(nameStartSingles[i], BT_NMSTRT);
		for (int i = 0; i < nameStartRanges.Length; i += 2)
			setCharType(nameStartRanges[i], nameStartRanges[i + 1],
						BT_NMSTRT);
		setCharType('\uD800', '\uDBFF', BT_LEAD4);
		setCharType('\uDC00', '\uDFFF', BT_MALFORM);
		setCharType('\uFFFE', '\uFFFF', BT_NONXML);
		int[] other = new int[256];
		for (int i = 0; i < 256; i++)
			other[i] = BT_OTHER;
		for (int i = 0; i < 256; i++)
			if (charTypeTable[i] == null)
				charTypeTable[i] = other;
		Array.Copy(asciiTypeTable, 0, charTypeTable[0], 0, 128);
	}

	/**
         * Returns the minimum number of bytes required to represent a single
         * character in this encoding.  The value will be 1, 2 or 4.
         */
	public int MinBytesPerChar
	{
		get { return _minBytesPerChar; }
	}
}

#pragma warning restore