// Copyright (c) 2003-2008 by AG-Software

namespace XmppSharp.XpNet;

public enum TokenType
{
	/// <summary>
	/// Represents one or more characters of data.
	/// </summary>
	DATA_CHARS,

	/// <summary>
	/// Represents a newline (CR, LF or CR followed by LF) in data.
	/// </summary>
	DATA_NEWLINE,

	/// <summary>
	/// Represents a complete start-tag <c>&lt;name&gt;</c> that doesn't have any attribute specifications.
	/// </summary>
	START_TAG_NO_ATTS,

	/// <summary>
	/// Represents a complete start-tag <c>&lt;name att="val"&gt;</c> that contains one or more attribute specifications.
	/// </summary>
	START_TAG_WITH_ATTS,

	/// <summary>
	/// Represents an empty element tag <c>&lt;name/&gt;</c>,
	/// that doesn't have any attribute specifications.
	/// </summary>
	EMPTY_ELEMENT_NO_ATTS,

	/// <summary>
	/// Represents an empty element tag <c>&lt;name att="val"/&gt;</c>, that contains one or more
	/// attribute specifications.
	/// </summary>
	EMPTY_ELEMENT_WITH_ATTS,

	/// <summary>
	/// Represents a complete end-tag <c>&lt;/name&gt;</c>.
	/// </summary>
	END_TAG,

	/// <summary>
	/// Represents the start of a CDATA section <c>&lt;![CDATA[</c>
	/// </summary>
	CDATA_SECT_OPEN,

	/// <summary>
	/// Represents the end of a CDATA section <c>]]&gt;</c>
	/// </summary>
	CDATA_SECT_CLOSE,

	/// <summary>
	/// Represents a general entity reference.
	/// </summary>
	ENTITY_REF,

	/// <summary>
	/// Represents a general entity reference to a one of the predefined XML entities: <c>amp</c>, <c>lt</c>, <c>gt</c>, <c>quot</c>, <c>apos</c>.
	/// </summary>
	MAGIC_ENTITY_REF,

	/// <summary>
	/// Represents a numeric character reference (decimal or
	/// hexadecimal), when the referenced character is less
	/// than or equal to 0xFFFF and so is represented by a
	/// single char.
	/// </summary>
	CHAR_REF,

	/// <summary>
	/// Represents a numeric character reference (decimal or hexadecimal), when the referenced character is greater than 0xFFFF and so is represented by a pair of chars.
	/// </summary>
	CHAR_PAIR_REF,

	/// <summary>
	/// Represents a processing instruction.
	/// </summary>
	PI,

	/// <summary>
	/// Represents an XML declaration or text declaration (a
	/// processing instruction whose target is
	/// <c>xml</c>).
	/// </summary>
	XML_DECL,

	/// <summary>
	/// Represents a comment <c>&lt;!-- comment --&gt;</c><para>This can occur both in the prolog and in content.</para>
	/// </summary>
	COMMENT,

	/// <summary>
	/// Represents a white space character in an attribute value, excluding white space characters that are part of line boundaries.
	/// </summary>
	ATTRIBUTE_VALUE_S,

	/// <summary>
	/// Represents a parameter entity reference in the prolog.
	/// </summary>
	PARAM_ENTITY_REF,

	/// <summary>
	/// Represents whitespace in the prolog.
	/// The token contains one or more whitespace characters.
	/// </summary>
	PROLOG_S,

	/// <summary>
	/// Represents <c>&lt;!NAME</c> in the prolog.
	/// </summary>
	DECL_OPEN,

	/// <summary>
	/// Represents <c>&gt;</c> in the prolog.
	/// </summary>
	DECL_CLOSE,

	/// <summary>
	/// Represents a name in the prolog.
	/// </summary>
	NAME,

	/// <summary>
	/// Represents a name token in the prolog that is not a name.
	/// </summary>
	NMTOKEN,

	/// <summary>
	/// Represents <c>#NAME</c> in the prolog.
	/// </summary>
	POUND_NAME,

	/// <summary>
	/// Represents <c>|</c> in the prolog.
	/// </summary>
	OR,

	/// <summary>
	/// Represents a <c>%</c> in the prolog that does not start
	/// a parameter entity reference.
	/// This can occur in an entity declaration.
	/// </summary>
	PERCENT,

	/// <summary>
	/// Represents a <c>(</c> in the prolog.
	/// </summary>
	OPEN_PAREN,

	/// <summary>
	/// Represents a <c>)</c> in the prolog that is not
	/// followed immediately by any of
	///  <c>*</c>, <c>+</c> or <c>?</c>.
	/// </summary>
	CLOSE_PAREN,

	/// <summary>
	/// Represents <c>[</c> in the prolog.
	/// </summary>
	OPEN_BRACKET,

	/// <summary>
	/// Represents<c>]</c> in the prolog.
	/// </summary>
	CLOSE_BRACKET,

	/// <summary>
	/// Represents a literal (EntityValue, AttValue, SystemLiteral or
	/// PubidLiteral).
	/// </summary>
	LITERAL,

	/// <summary>
	/// Represents a name followed immediately by <c>?</c>.
	/// </summary>
	NAME_QUESTION,

	/// <summary>
	/// Represents a name followed immediately by <c>*</c>.
	/// </summary>
	NAME_ASTERISK,

	/// <summary>
	/// Represents a name followed immediately by <c>+</c>.
	/// </summary>
	NAME_PLUS,

	/// <summary>
	/// Represents <c>&lt;![</c> in the prolog.
	/// </summary>
	COND_SECT_OPEN,

	/// <summary>
	/// Represents <c>]]&gt;</c> in the prolog.
	/// </summary>
	COND_SECT_CLOSE,

	/// <summary>
	/// Represents <c>)?</c> in the prolog.
	/// </summary>
	CLOSE_PAREN_QUESTION,

	/// <summary>
	/// Represents <c>)*</c> in the prolog.
	/// </summary>
	CLOSE_PAREN_ASTERISK,

	/// <summary>
	/// Represents <c>)+</c> in the prolog.
	/// </summary>
	CLOSE_PAREN_PLUS,

	/// <summary>
	/// Represents <c>,</c> in the prolog.
	/// </summary>
	COMMA,
}
