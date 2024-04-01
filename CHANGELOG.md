# XMPP#
<img alt="Static Badge" src="https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=github&label=Github&link=https%3A%2F%2Fgithub.com%2Fnathan130200%2FXmppSharp">



A manipulation library and utility for XMPP inspired by the classic agsXMPP library (developed by [AG-Software](https://www.ag-software.net)). The main objective of this library is to reduce the level of unnecessary verbosity for constructing XML tags.

### Changelog

- 1.3.3: 
	- Make JID class not be immutable, if you need an immutable version, wrap inside a record.
	- Added `FirstChild` and `LastChild` in element class, for fast access first and last elements in current scope.
	- Added missing Multi User Chat namespace.
- 1.3.4:
	- Add support for both .NET 7 and .NET 8 versions.

- 1.4.0
	- **BREAKING CHANGES**
		- **Some types renamed or will be moved to another files/namespaces!**
	- Added target support for .NET 6.
	- Add service discovery elements.
	- Enhance JID construction.
	- Make JID records types again, but keep setters instead init-only.
	- `Jid.Empty` now provide a empty jid instance.
	- `Jid(string jid)` contructor will try parse as full jid before set domain component.
	- Added `ReadOnlyJid` that wraps Jid instance into a read-only struct for immutable/exposing.
	- Added `ReadOnlyJid.Empty` static helper to fast create its instance.
	- `ReadOnlyJid` has proper ways to cast between `Jid` and other `ReadOnlyJid` instances.
	- Added helper method to convert `XElement` objects into Xmpp Sharp `Element` objects.
	- Begin add XML docstrings in types/methods/fields/properties etc.

- 1.4.1
	- Fix NRE while setting attribute with `SetAttributeValue`