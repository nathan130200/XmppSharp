# XMPP#
A manipulation library and utility for XMPP inspired by the classic agsXMPP library (developed by [AG-Software](https://www.ag-software.net)). The main objective of this library is to reduce the level of unnecessary verbosity for constructing XML tags.

### Changelog

- 1.1.0: Initial commit, added minimum classes to manipulate XMPP elements.
- 1.2: 
- 1.3.0: Fix jid wrong parsing in domain part and some improvements.
- 1.3.2: Added dataforms support.
- 1.3.3: 
	- Make JID class not be immutable, if you need an immutable version, wrap inside a record.
	- Added `FirstChild` and `LastChild` in element class, for fast access first and last elements in current scope.
	- Added missing Multi User Chat namespace.
- 1.3.4:
	- Add support for both .NET 7 and .NET 8 versions.