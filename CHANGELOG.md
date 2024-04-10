# XMPP#

[![github](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=github&label=Github)](https://github.com/nathan130200/XmppSharp) [![nuget](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=nuget&label=NuGet&color=blue)](https://github.com/nathan130200/XmppSharp)

A manipulation library and utility for XMPP inspired by the classic agsXMPP library (developed by [AG-Software](https://www.ag-software.net)). The main objective of this library is to reduce the level of unnecessary verbosity for constructing XML tags.

# Changelog

### 1.4.0
	
- **Breaking changes**: Some types renamed or will be moved to another files/namespaces!
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

### 1.4.1
	
- Fix NRE while setting attribute with `SetAttributeValue`

### 1.4.2
	
- Minor changes & fixes.
- Added helper methods to fast get functions to parse primitive types.

### 2.0.0
	
- v2.0 is here! I was finally able to migrate to an improved system using `XElement`.
	- First of all, removed ALL AI-generated docstrings. I think it was one of the most bizarre ideas I've ever had 😂 and it didn't work out very well, it generated a lot of redundant things that weren't well explained. Then soon I will calmly document everything.
	- Some things **_may_** break! For example, how to create/declare your elements. But I did everything I could to maintain the compatibility layer between old `Element` class and `XElement` class!
	- By the way, most of the xmpp elements remained the same, the way to access/define data also remained almost the same.
	- `CDATA` and `Comment` nodes are supported now when parsing (Even though it is not recommended to use these types of nodes in the XMPP protocol standards, this depends on each developer and the needs of each project.).
	- **Don't forget to report bugs on github! Even though I personally use it for my own gaming XMPP server, there may be some bug that goes unnoticed.**