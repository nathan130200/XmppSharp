# XMPP#

[![github](https://img.shields.io/badge/XMPP%23_%20Core-ffe000?style=flat-square&logo=github&label=Github)](https://github.com/nathan130200/XmppSharp)

A manipulation library and utility with the main objective to reduce the level of unnecessary verbosity for constructing XML tags for XMPP protocol, supporting .NET 7.0+


## Common Used Types
- `XmppSharp.Jid`
- `XmppSharp.Namespaces`
- `XmppSharp.XmppEnum`
- `XmppSharp.Dom.Element`
- `XmppSharp.Factory.ElementFactory`
- `XmppSharp.Parser.XmppParser`
- `XmppSharp.Parser.XmppParser`
- `XmppSharp.Parser.XmppStreamParser`
- `XmppSharp.Parser.XmppBufferedStreamParser`
- `XmppSharp.Protocol.Base.StreamStream`
- `XmppSharp.Protocol.Base.StreamFeatures`
- `XmppSharp.Protocol.Base.StreamError`
- `XmppSharp.Protocol.Base.StanzaError`
- `XmppSharp.Protocol.Iq`
- `XmppSharp.Protocol.IqType`
- `XmppSharp.Protocol.Message`
- `XmppSharp.Protocol.MessageType`
- `XmppSharp.Protocol.Presence`
- `XmppSharp.Protocol.PresenceType`
- `XmppSharp.Protocol.PresenceShow`

## Parsers implementations
- There are built-in parsers implemented:
	- `XmppStreamReader` based on `XmlReader`
	- `XmppStreamParser` based on [agsXMPP](https://www.ag-software.net/agsxmpp-sdk/) parser.
	- `XmppTokenizer` based on `XmppStreamParser` and provides an expat like parsing events (`OnStartTag`, `OnEndTag`, `OnText`, `OnCdata`, `OnComment`).

- Alternative Official XMPP Parsers:
	- `XmppSharp.Expat` provide an implementation of __XMPP#__ parsing wrapper using [libexpat](https://github.com/libexpat/libexpat). Available on [NuGet](https://www.nuget.org/packages/XmppSharp.Expat/).