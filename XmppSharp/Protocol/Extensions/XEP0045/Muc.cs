using Jabber.Attributes;
using System.Xml;
using XmppSharp.Abstractions;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0045;

[StanzaExtension("x", Namespaces.Muc)]
public sealed class Muc : StanzaExtension
{
    public Muc()
    {

    }

    internal Muc(Element element) : base(element)
    {
        if (element.Element("history") is not { } history) return;

        History = new()
        {
            MaxChars = history.Attribute("maxchars"),

            MaxStanzas = history.Attribute("maxstanzas"),

            Seconds = history.Attribute("seconds"),

            Since = history.Attribute("since") is { } since
                ? XmlConvert.ToDateTimeOffset(since)
                : null
        };
    }

    public History? History { get; set; }

    public string? Password { get; set; }

    public override Element Serialize()
    {
        var result = new Element("x", Namespaces.Muc);

        if (Password is { } pwd)
            result.SetTag("password", Namespaces.Muc, value: pwd);

        if (History is { } state)
        {
            result.C("history", x =>
            {
                x.SetAttribute("maxchars", state.MaxChars);

                x.SetAttribute("maxstanzas", state.MaxStanzas);

                x.SetAttribute("seconds", state.Seconds);

                if (state.Since is { } since)
                    x.SetAttribute("since", XmlConvert.ToString(since));
            });
        }

        return result;
    }
}