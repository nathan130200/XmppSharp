# XMPP#
Lightweight XMPP library.

[![nuget](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/XmppSharp/)

[![nuget-expat](https://img.shields.io/badge/XmppSharp.Expat-1?style=plastic&logo=nuget&label=NuGet&color=green)](https://www.nuget.org/packages/XmppSharp.Expat/)

[![nuget-tokenizer](https://img.shields.io/badge/XmppSharp.Tokenizer-1?style=plastic&logo=nuget&label=NuGet&color=orange)](https://www.nuget.org/packages/XmppSharp.Tokenizer/)

____

#### Supported features

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

#### Parsing

After XMPP# v6.0 we are using [libexpat](https://github.com/libexpat/libexpat/) as default XML parser for xmpp protocol. But additional parsers can be implemented along with the lib.

You can still use older XMPP# version that includes builtin XMPP parser using `XmlReader` or use <i>XMPP# Tokenizer</i> package.

The class `XmppSharp.Dom.XmppDocument` was keep to provide basic XML parsing from file or string in a simple and fast way using .NET's `XmlReader`.

#### Element Factory

The `XmppElementFactory` class registers and maintains the mapping of all XML elements to their respective classes. Each element is qualified by a name (Tag name) and its namespace URI, when registered the parser will obtain this information and construct the class corresponding to the XML element. If the mapping does not exist, it will use a fallback by constructing an instance of `XmppSharp.Dom.XmppElement`.

Consider the example table below demonstrating how this mapping works:

| Qualified Tag Name | Namespace(s) | Mapped Class |
| ------------------ | ------------ | ------------ |
stream:stream|http://etherx.jabber.org/streams|XmppSharp.Protocol.Base.StreamStream
stream:features|http://etherx.jabber.org/streams|XmppSharp.Protocol.Base.StreamFeatures
starttls|urn:ietf:params:xml:ns:xmpp-tls|XmppSharp.Protocol.Tls.StartTls
auth|urn:ietf:params:xml:ns:xmpp-sasl|XmppSharp.Protocol.Sasl.Auth
success|urn:ietf:params:xml:ns:xmpp-sasl|XmppSharp.Protocol.Sasl.Success
iq|*see note below*|XmppSharp.Protocol.Iq
message|*see note below*|XmppSharp.Protocol.Message
presence|*see note below*|XmppSharp.Protocol.Presence
error|*see note below*|XmppSharp.Protocol.Base.StanzaError
...|...|...

> [!NOTE]
> Some cases like `iq`, `message`, `presence` have more than one namespace defined (because it depends on each specific namespace URI), but they are declared in the same way. The difference is that more than one namespace is assigned to this element, so the `XmppElementFactory` can correctly map which one it will instantiate.

<hr/>

Defining your XML elements is simple, just create a class that inherits the base class `XmppSharp.Dom.XmppElement` or similar, call the base constructor of the class to initialize the tag name correctly. And add the attribute `[XmppTag(tagName, namespace)]` for each desired tag and namespace and register the type by calling `XmppElementFactory.RegisterType` (or to register an entire assembly `XmppElementFacotry.RegisterAssembly`), eg:

```cs
// Declare namespace constant somewhere.
const string XMLNS_TODOLIST = "urn:todolist:app";

[XmppTag("todo_item", XMLNS_TODOLIST)]
public class TodoItem : XmppElement
{
    // Call base constructor to setup this element instance.
    public TodoItem() : base("todo_item", XMLNS_TODOLIST)
    {
    }

    // Define other properties for new custom element.

    public bool HasDone
    {
        get => HasTag("done");
        set 
        {
            if(!value) RemoveTag("done");
            else SetTag("done");
        }
    }

    public string? Name
    {
        get => GetAttribute("name");
        set => SetAttribute("name", value);
    }
}
```

<hr/>

Due internal reasons we merged expat parser into main package. We no longer provide default parser using XmlReader