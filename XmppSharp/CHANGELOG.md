# XMPP#

[![github](https://img.shields.io/badge/XMPP%23_%20Core-ffe000?style=flat-square&logo=github&label=Github)](https://github.com/nathan130200/XmppSharp)

A manipulation library and utility with the main objective to reduce the level of unnecessary verbosity for constructing XML tags for XMPP protocol, supporting `net6.0`, `net7.0` and `net8.0`.

### Version History

*3.0.0*

- Most big change! Bring back own XML implementation supporting nodes:
	- `Element`
	- `Text`
	- `Comment`
	- `Cdata`

____

*3.1.0*
- Add enhanced utilities & helper methods to interact with element and nodes.

____

*3.1.1*

- Minor fixes.
- Fixed wrong sub classing around some elements.
- Added full control about XML formatting.
- In .Net6 use `TryParseHelper` helper methods to parse attribute values. While in .Net7 or higher use `IParsable<T>` abstract static interfaces feature, for parsing attribute values.

____

*3.1.2*

- Added missing `StartTag` and `EndTag` in elements. Both strings will contains well-formed XML string.
- Ability to make a shallow copy of element instead a full copy with `Element.Clone(deep)` overload.
- More formatting options in `XmlFormatting` structure.

____

*3.1.3*

- Enhance XMPP parser with different ctors provide an Stream or an factory function to create an stream. `XmppParser::Reset()` no longer needs an stream as argument.
- Renamed `XmppParser::Advance` to `XmppParser::AdvanceAsync` for async version and leave `XmppParser::Advance` for sync method version.
- Added `GetAwaiter` in XMPP parser for simple calling `await myParser;` have same behaviour and return same result as calling `await myParser.AdvanceAsync()`

____


*3.1.4*

- Minor fixes around `XmppParser` and added helper method to advance and get next element.

____

*3.1.5*

- Rename `XmppParser` to `DefaultXmppParser` to indicade this uses regular .NET `XmlReader` to parse xmpp packets.
- Add basic abstraction layer to implement your own xmpp parser. Also i'm releasing a separated package `XmppSharp.Expat` to provide expat XMPP parser implementation. (Note: You must install native libraries to use expat.
- Added `AsyncHelper` (from `AspNetCore` repo) to calling async functions in sync methods.

> In **XMPP#** repository i did an github actions to automatically build expat using vcpkg with most common systems: ubuntu, macos, windows but only x64 is working at this moment). Consider using [XmppShap.Expat](https://www.nuget.org/packages/XmppSharp.Expat/) package too if you need a fast and stable parser.

____

*3.1.6*

- Minor improvements.
- Fixed wrong indent chars & side for default formatting options.
- Fixed `Element.Value` returning entire inner text from all descendant nodes.