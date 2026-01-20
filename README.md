# XMPP#
Lightweight XMPP library.

[![nuget](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/XmppSharp/)

____

#### Supported element types.

- [RFC-6120](https://xmpp.org/rfcs/rfc6120.html) - Core
- [XEP-0004](https://xmpp.org/extensions/xep-0004.html) - Data Forms
- [XEP-0012](https://xmpp.org/extensions/xep-0012.html) - Last Activity
- [XEP-0030](https://xmpp.org/extensions/xep-0030.html) - Service Discovery
- [XEP-0045](https://xmpp.org/extensions/xep-0045.html) - Multi User Chat
- [XEP-0047](https://xmpp.org/extensions/xep-0047.html) - In-Band Bytestreams
- [XEP-0085](https://xmpp.org/extensions/xep-0085.html) - Chat States Notifications
- [XEP-0092](https://xmpp.org/extensions/xep-0092.html) - Software Version
- [XEP-0172](https://xmpp.org/extensions/xep-0172.html) - User Nickname
- [XEP-0199](https://xmpp.org/extensions/xep-0199.html) - Ping
- [XEP-0202](https://xmpp.org/extensions/xep-0202.html) - Entity Time
- [XEP-0203](https://xmpp.org/extensions/xep-0203.html) - Delayed Delivery

____

#### Element Factory

The `XmppElementFactory` class registers and maintains the mapping of all XML elements to their respective classes. Each element is qualified by a name (Tag name) and its namespace URI, when registered the parser will obtain this information and construct the class corresponding to the XML element. If the mapping does not exist, it will use a fallback by constructing an instance of `XmppSharp.Dom.XmppElement`.

Consider the example table below demonstrating how this mapping works:

| Qualified Tag Name | Namespace(s) | Mapped Class |
| ------------------ | ------------ | ------------ |
stream:stream|http://etherx.jabber.org/streams|XmppSharp.Protocol.Base.StreamStream
starttls|urn:ietf:params:xml:ns:xmpp-tls|XmppSharp.Protocol.Tls.StartTls
auth|urn:ietf:params:xml:ns:xmpp-sasl|XmppSharp.Protocol.Sasl.Auth

> [!NOTE]
> Some cases (like `iq`, `message`, `presence`) have more than one namespace defined (because it depends on each specific side of connection), but they are declared in the same way. The difference is that more than one namespace is assigned to this element, so the `XmppElementFactory` can correctly map which one it will instantiate.
