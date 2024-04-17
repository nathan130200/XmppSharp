# XMPP#

A manipulation library and utility with the main objective to reduce the level of unnecessary verbosity for constructing XML tags for XMPP protocol.

<hr/>

### XMPP Parser

Going back to the roots of .NET in **XMPP#** we use `XmlReader` as an XMPP interpreter, it works perfectly, we are able to apply encryption and compression to the network streams and underlying streams, and it validates/encodes the values in the well-formed XML rules.

### Element Factory

We have the Element Factory, a class where you can register all XML elements mapped to the respective classes, and thus, when reading the XML, the parser will convert it to the desired element.

An example of this, when receiving the stanzas `iq`, `message` or `presence` they will be automatically converted and treated as `XmppSharp.Protocol.Iq`, `XmppSharp.Protocol.Message` or `XmppSharp.Protocol.Presence` making it easier to reading the code and implementing new features.

You can also implement a parser that meets your project requirements and also invoke Element Factory to create the elements, which will return the instance of the correct element type.

### Element Types

Using the same technique to declare the elements.

1. Creates a class, with a public constructor without parameters, that calls the base class `XElement` (needs to inherit the `XElement` class). 
1. In the base constructor, put the name of the element + namespace, if the element is qualified by prefix, also add the prefix, declaring as `new XAttribute(XNamespace.Xmlns + prefix, namespaceURI)`.
1. Add the attribute `[XmppTag(localName, namespace)]` to the class to be detectable by `ElementFactory`. Note that this attribute can be inserted multiple times, precisely to treat elements that are the same but may have different namespaces.
1. And finally, call `ElementFactory.RegisterType` to add the type of the element to the registry with all the names and namespaces declared by the `XmppTag` attribute. Or if you prefer `ElementFactory.RegisterTypes(assembly)` to register all element types in the specified assembly.

**Check in [wiki](https://github.com/nathan130200/XmppSharp/wiki/Custom-Element-Type) for examples.**