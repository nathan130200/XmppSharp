using Jabber.Abstractions;
using Jabber.Dom;
using Jabber.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace Jabber.Protocol.Base;

public abstract class Stanza : IXmppSerializable
{
    public string? Id { get; set; }
    public Jid? From { get; set; }
    public Jid? To { get; set; }

    List<StanzaExtension>? _extensions;

    public void SwitchDirection()
    {
        (From, To) = (To, From);
    }

    public Stanza()
    {

    }

    public Stanza(Element element)
    {
        ArgumentNullException.ThrowIfNull(element);

        Id = element.GetAttribute("id");

        From = element.GetAttribute("from");

        To = element.GetAttribute("to");
    }

    public bool FindExtension<TExtension>([NotNullWhen(true)] out TExtension? result) where TExtension : StanzaExtension
    {
        result = GetExtension<TExtension>();

        return result != null;
    }

    public TExtension? GetExtension<TExtension>() where TExtension : StanzaExtension
    {
        lock (this)
        {
            if (_extensions is null) return null;

            foreach (var ext in _extensions)
            {
                if (ext is TExtension result)
                    return result;
            }
        }

        return null;
    }

    public TExtension AddExtension<TExtension>() where TExtension : StanzaExtension
    {
        lock (this)
        {
            _extensions ??= [];

            if (GetExtension<TExtension>() is not { } result)
            {
                result = StanzaExtensionFactory.Create<TExtension>();

                _extensions.Add(result);
            }

            return result;
        }
    }

    public void AddExtension<TExtension>(Action<TExtension> callback) where TExtension : StanzaExtension
    {
        ArgumentNullException.ThrowIfNull(callback);

        callback(AddExtension<TExtension>());
    }

    internal StanzaExtension RegisterExtensionFromElement(Element element)
    {
        lock (this)
        {
            _extensions ??= [];

            var se = StanzaExtensionFactory.Create(element);

            _extensions.Add(se);

            return se;
        }
    }

    public void RemoveAllExtensions()
    {
        lock (this)
        {
            if (_extensions is null) return;

            _extensions.Clear();
        }
    }

    public abstract Stanza Clone();

    public abstract Element Serialize();
}
