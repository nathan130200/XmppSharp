# XMPP#
Lightweight XMPP library.

[![nuget](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/XmppSharp/)

<hr/>

#### Supported features

- [RFC-6120](https://xmpp.org/rfcs/rfc6120.html) - Core
- [XEP-0004](https://xmpp.org/extensions/xep-0004.html) - Data Forms
- [XEP-0012](https://xmpp.org/extensions/xep-0012.html) - Last Activity
- [XEP-0030](https://xmpp.org/extensions/xep-0030.html) - Service Discovery
- [XEP-0045](https://xmpp.org/extensions/xep-0045.html) - Multi User Chat
- [XEP-0047](https://xmpp.org/extensions/xep-0047.html) - In-Band Bytestreams
- [XEP-0085](https://xmpp.org/extensions/xep-0085.html) - Chat States Notifications
- [XEP-0172](https://xmpp.org/extensions/xep-0172.html) - User Nickname
- [XEP-0199](https://xmpp.org/extensions/xep-0199.html) - Ping
- [XEP-0202](https://xmpp.org/extensions/xep-0202.html) - Entity Time
- [XEP-0203](https://xmpp.org/extensions/xep-0203.html) - Delayed Delivery

<hr/>

#### Parsing
The lib uses `XmlReader` as a base to read and tokenize the XML stream for the XMPP protocol. It can control and maintain the XML state in a stable and secure way.

Additional parsers can be implemented along with the lib. For example, the default implementation of `XmppSharp.Expat` contains the basic code for XML parsing using the expat parser (basically one of the best XML parsers out there).

Consider that some parsers (like our official implementation of `XmppSharp.Expat`) use external libraries, requiring P/Invoking, which requires the user to have already installed the library on their computer. The most practical way to obtain these compiled libraries is using [vcpkg](https://vcpkg.io/) (in the case of expat, they have releases ready for download in the [official repository](https://github.com/libexpat/libexpat/)).

In the current version we introduced a new class `XmppSharp.Dom.Document` that aims to parse an XML file or even an XML string in a simple and fast way without also depending on `System.Xml` using fully the Xmpp Sharp APIs! It supports both reading and writing an XML document to a file or string.


#### Element Factory

The `ElementFactory` class registers and maintains the mapping of all XML elements to their respective classes. Each element is qualified by a name (Tag name) and its namespace URI, when registered the parser will obtain this information and construct the class corresponding to the XML element. If the mapping does not exist, it will use a fallback by constructing an instance of `XmppSharp.Dom.Element`.

Consider the example table below demonstrating how this mapping works:

| Qualified Tag Name | Namespace(s) | Mapped Class |
| ------------------ | ------------ | ------------ |
iq|*see below*|XmppSharp.Protocol.Iq
message|*see below*|XmppSharp.Protocol.Iq
presence|*see below*|XmppSharp.Protocol.Iq
error|*see below*|XmppSharp.Protocol.Base.StanzaError
starttls|urn:ietf:params:xml:ns:xmpp-tls|XmppSharp.Protocol.Tls.StartTls
success|urn:ietf:params:xml:ns:xmpp-sasl|XmppSharp.Protocol.Sasl.Success
stream:features|http://etherx.jabber.org/streams|XmppSharp.Protocol.Base.StreamFeatures
...|...|...

> [!NOTE]
> Some cases like `iq`, `message`, `presence`, `error` (error element inside an stanza), have more than one namespace defined (because it depends on each specific piece of the protocol), but they are declared in the same way. The difference is that more than one namespace is assigned to this element, so the `ElementFactory` can correctly map which one it will instantiate.

<hr/>

Defining your XML elements is simple, just create a class that inherits the base class `XmppSharp.Dom.Element` or similar, call the base constructor of the class to initialize the tag name correctly. And add the attribute `[XmppTag(tagName, namespace)]` for each desired tag and namespace and register the type by calling `ElementFactory.RegisterType` (or to register an entire assembly `ElementFacotry.RegisterAssembly`), eg:

```cs
// define xmpp tags
[XmppTag("bind", Namespaces.Bind)]
public class Bind : Element
{
    public Bind() : base("bind", Namespaces.Bind) // call base constructor to set-up this element instance.
    {
        ...
    }
}
```
