# XMPP#

A manipulation library and utility with the main objective to reduce the level of unnecessary verbosity for constructing XML tags for XMPP protocol.

 [![nuget](https://img.shields.io/badge/XmppSharp-1?style=plastic&logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/XmppSharp/)

<hr/>

### XMPP Parser

Going back to the roots of .NET in **XMPP#** we use `XmlReader` as an XMPP interpreter, it works perfectly, we are able to apply encryption and compression to the network streams and underlying streams, and it validates/encodes the values in the well-formed XML rules.

### Element Factory

We have the Element Factory, a class where you can register all XML elements mapped to the respective classes, and thus, when reading the XML, the parser will convert it to the desired element.

An example of this, when receiving the stanzas `iq`, `message` or `presence` they will be automatically converted and treated as `XmppSharp.Protocol.Iq`, `XmppSharp.Protocol.Message` or `XmppSharp.Protocol.Presence` making it easier to reading the code and implementing new features.

You can also implement a parser that meets your project requirements and also invoke Element Factory to create the elements, which will return the instance of the correct element type.

### Element Types

Check in [wiki](https://github.com/nathan130200/XmppSharp/wiki/Custom-Element-Type) for a basic tutorial how to declare custom elements in **XMPP#**.