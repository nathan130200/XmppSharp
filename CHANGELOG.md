# XMPP#

[![github](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=github&label=Github)](https://github.com/nathan130200/XmppSharp)

A manipulation library and utility with the main objective to reduce the level of unnecessary verbosity for constructing XML tags for XMPP protocol, supporting `net6.0`, `net7.0` and `net8.0`.

# Lastest Updates

**3.0.0**
____
- Most big change! Bring back own XML implementation supporting nodes:
	- `Element`
	- `Text`
	- `Comment`
	- `Cdata`

**3.1.0**
- Add enhanced utilities & helper methods to interact with element and nodes.

**3.1.1**
____
- Minor fixes.
- Fixed wrong sub classing around some elements.
- Added full control about XML formatting.
- In .Net6 use `TryParseHelper` helper methods to parse attribute values. While in .Net7 or higher use `IParsable<T>` abstract static interfaces feature, for parsing attribute values.

**3.1.2**
____
- Added missing `StartTag` and `EndTag` in elements. Both strings will contains well-formed XML string.
- Ability to make a shallow copy of element instead a full copy with `Element.Clone(deep)` overload.
- More formatting options in `XmlFormatting` structure.

**3.1.3**
____
- Enhance XMPP parser with different ctors provide an Stream or an factory function to create an stream. `XmppParser::Reset()` no longer needs an stream as argument.