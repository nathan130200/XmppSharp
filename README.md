# XMPP#
A manipulation library and utility for XMPP inspired by the classic agsXMPP library (developed by [AG-Software](https://www.ag-software.net))

The main objective of this library is to reduce the level of unnecessary verbosity for constructing XML tags.

It doesn't necessarily need to be strict. There are many headaches when manipulating XmlElement (System.Xml) and XElement (System.Xml.Linq) elements due to the exaggerated level of complexity.

- `System.Xml.XmlElement` requires you to have an `XmlDocument` as root node and everything can only be changed there.

- `System.Xml.Linq.XElement` requires a lot of unnecessary complexity to manage element names and namespaces.

So with that in mind, I bring the functionality of the [agsXMPP](https://www.ag-software.net/agsxmpp-sdk/) features back to a modern .NET with some improvements!

### The Parser

Going back to the roots of .NET in **XMPP#** we use `XmlReader` as an XMPP interpreter, it works perfectly, we are able to apply encryption and compression to the network streams and underlying streams, and it validates/encodes the values in the well-formed XML rules.

Of course, if it is your need or the scope of your project, you can implement an external parser like Expat that works perfectly.

### Element Factory

Just like agsXMPP, we again have the Element Factory, a class where you can register all XML elements mapped to the respective classes, and thus, when reading the XML, the parser will convert it to the desired element.

An example of this, when receiving the stanzas `iq`, `message` or `presence` they will be automatically converted and treated as `XmppSharp.Protocol.Iq`, `XmppSharp.Protocol.Message` or `XmppSharp.Protocol.Presence` making it easier to reading the code and implementing new features.

You can also implement a parser that meets your project requirements and also invoke Element Factory to create the elements, which will return the instance of the correct element type.

### Element Types

- To declare an element type, the first thing is to annotate the class with the `XmppTag` one or many attribute. Where you will receive two parameters of the element's local name and the element's namespace. This attribute is the information that will be used to register the element in the `Element Factory`.
- The class must inherit from the `XmppSharp.Xmpp.Dom.Element` type. And you must call the base constructor with the same parameters used in the `XmppTag` attribute.
- In some cases, such as the IQ stanza, it may have several namespaces (`jabber:client`, `jabber:server`, `jabber:component:connect` and `jabber:component:accept`), in this case you must only declare it in the `XmppTag` attribute, however in the base constructor you can optionally provide the namespace or not, when reading the XMP it will automatically fill in the namespace in the current scope.

### Element Types: Examples

See below some examples of how to declare elements. Oh and don't forget to register the elements! The element factory class has an auxiliary method that searches the entire assembly for elements that are annotated with the XmppTag attribute and that are of the base type Element.


#### Ping query.

```cs

// In attribute you must provide ONLY local name of the element.
// So if your element is qualified by a prefix and namespace, you must provide local name AND the namespace of prefix.
// Check the Stream Features element for more details.
[XmppTag("ping", "urn:xmpp:ping")]
public class Ping : Element
{
	// In base ctor, you can provide a full qualified element name!
	public Ping() : base("ping", "urn:xmpp:ping")
	{
		
	}
}
```

#### Bind feature.

```cs
[XmppTag("bind", Namespace.Bind)]
public class Bind : Element
{
    public Bind() : base("bind", Namespace.Bind)
    {

    }

    public string Resource
    {
        get => GetTag("resource");
        set
        {
            if (value == null)
                RemoveTag("resource");
            else
                SetTag("resource", value);
        }
    }

    public Jid Jid
    {
        get
        {
            var jid = GetTag("jid");

            if (Jid.TryParse(jid, out var result))
                return result;

            return null;
        }
        set
        {
            if (value == null)
                RemoveTag("jid");
            else
                SetTag("jid", value.ToString());
        }
    }
}

```

#### Stream features element.

```cs
// Stream features element has...
//  a prefix = stream
//  a namespace of prefix stream = http://etherx.jabber.org/streams
[XmppTag("features", Namespace.Stream)]
public class Features : Element
{
    // And again provide qualified name in ctor.
    public Features() : base("stream:features", Namespace.Stream)
    {

    }

    public Mechanisms Mechanisms
    {
        get => Child<Mechanisms>();
        set => ReplaceChild<Mechanisms>(value);
    }

    public StartTls StartTls
    {
        get => Child<StartTls>();
        set => ReplaceChild(value);
    }

    public Bind Bind
    {
        get => Child<Bind>();
        set => ReplaceChild(value);
    }

    public Session Session
    {
        get => Child<Session>();
        set => ReplaceChild(value);
    }

    public bool SupportsStartTls
        => StartTls != null;

    public bool SupportsBind
        => Bind != null;

    public bool SupportsSaslAuth
        => Mechanisms != null;
}
```