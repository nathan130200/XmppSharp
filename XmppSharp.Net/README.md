# XMPP#Net
Client and component connection implementations using [XMPP#](https://github.com/nathan130200/XmppSharp).

----

This package contains the implementation of the [client](https://xmpp.org/rfcs/rfc6120.html) and [components](https://xmpp.org/extensions/xep-0114.html) protocols. The goal is to split this package for specific uses. 

For example, XMPP# itself is a library that implements classes, functions, etc., for use with the XMPP protocol itself, so the additional packages are extensions for this.

This package also implements the SASL factory and mechanisms (authentication) for client connections.


### Register custom SASL handler

To register a SASL handler or overwrite an existing one, simply follow these steps:

1. Create the class that extends the base class `XmppSaslHandler`.

```cs
public class SaslPlainHandler : XmppSaslHandler
{

}
```

2. Override the `Init` method to send the initial authentication packet:

```cs
protected internal override void Init()
{
    var buf = new StringBuilder()
        .Append('\0')
        .Append(Connection.User)
        .Append('\0')
        .Append(Connection.Password)
        .ToString()
        .GetBytes();

    var el = new Auth
    {
        Mechanism = "PLAIN",
        Value = Convert.ToBase64String(buf)
    };

    Connection.Send(el);
}
```

> In the case of the PLAIN mechanism, there are only 2 fields sent `nullchar + user + nullchar + password`.

3. Override the `Invoke` method to handle xmpp elements in SASL handler:

```cs
protected internal override SaslHandlerResult Invoke(XmppElement e)
{
    // server responded with success.
    if (e is Success)
        return SaslHandlerResult.Success;

    // server responded with failure.
    if (e is Failure failure)
        return SaslHandlerResult.Failure(failure.Condition, failure.Text);

    // is not SASL element, skip.
    return SaslHandlerResult.Continue;
}
```

This is how the PLAIN mechanism is implemented on the client. And registering is simple. Just call the function: `RegisterMechanism("PLAIN", c => new XmppSaslPlainHandler(c));`

----

There are a few things to consider in this `Invoke` method.

- It expects to return a `SaslHandlerResult` that tellswhether the authentication failed, succeeded, or ignored.

- In the case of ignored, it means that the element was not processed by the SASL handler (it does not belong to the mechanism). If it is ignored, it will be sent to the `OnElement` event of the connection.
