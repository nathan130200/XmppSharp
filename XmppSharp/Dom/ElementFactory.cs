using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using XmppSharp.Attributes;
using XmppSharp.Collections;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Core;

namespace XmppSharp.Dom;

/// <summary>
/// Delegate helper that helps build complex elements that are not registered in the element factory.
/// </summary>
/// <param name="parent">Parent element to which this element will belong.</param>
/// <param name="tagName">Qualified tag name of the element.</param>
/// <param name="namespaceURI">Namespace that qualifies this element.</param>
/// <returns>Returns null if the element cannot be resolved or the instance of <see cref="XmppSharp.Dom.Element" /> containing the element is constructed.</returns>
public delegate Element? ElementFactoryResolver(string tagName, string? namespaceURI, Element? parent);

public static class ElementFactory
{
    /// <summary>
    /// Global event that will try to build a complex element.
    /// </summary>
    /// <remarks>
    /// The order of construction of Element Factory elements is outlined below:
    /// <list type="number">
    /// <item>Special elements such as <see cref="StreamStream"/>, <see cref="StreamError"/>, <see cref="Iq"/>, <see cref="Message"/> or <see cref="Presence"/>.</item>
    /// <item>Elements registered with <see cref="RegisterAssembly"/> or <see cref="RegisterType"/></item>
    /// <item>Invoking the global event <see cref="ResolveElement"/> in the final attempt to construct the element.</item>
    /// </list>
    /// </remarks>
    public static event ElementFactoryResolver ResolveElement
    {
        add
        {
            ThrowHelper.ThrowIfNull(value);

            lock (_callbacks)
                _callbacks.Add(value);
        }
        remove
        {
            ThrowHelper.ThrowIfNull(value);

            lock (_callbacks)
                _callbacks.Remove(value);
        }
    }

    static readonly List<ElementFactoryResolver> _callbacks = new();
    static readonly ConcurrentDictionary<Type, IEnumerable<XmppTag>> s_ElementTypes = new();
    static readonly IEnumerable<XmppTag> s_Empty = Enumerable.Empty<XmppTag>();

    static ElementFactory()
    {
        RegisterAssembly(typeof(ElementFactory).Assembly);
    }

    public static void RegisterType<T>()
        => RegisterType(typeof(T));

    public static void RegisterType(Type type)
    {
        ThrowHelper.ThrowIfNull(type);

        var tags = from a in type.GetCustomAttributes<XmppTagAttribute>()
                   select new XmppTag(a.TagName, a.NamespaceURI);

        if (!tags.Any())
            return;

        if (!s_ElementTypes.TryGetValue(type, out var current))
            s_ElementTypes[type] = tags;
        else
            s_ElementTypes[type] = current.Concat(tags);
    }

    public static void RegisterAssembly(Assembly assembly)
    {
        ThrowHelper.ThrowIfNull(assembly);

        var elements = from type in assembly.GetTypes()
                       where !type.IsAbstract && type.IsSubclassOf(typeof(Element))
                       let tags =
                            from attr in type.GetCustomAttributes<XmppTagAttribute>()
                            select new XmppTag(attr.TagName, attr.NamespaceURI)
                       where tags.Any()
                       select new { type, tags };

        foreach (var it in elements)
        {
            if (!s_ElementTypes.TryGetValue(it.type, out var current))
                s_ElementTypes[it.type] = it.tags;
            else
                s_ElementTypes[it.type] = current.Concat(it.tags);
        }
    }

    public static IEnumerable<XmppTag> LookupTagsFromType<T>() => LookupTagsFromType(typeof(T));

    public static IEnumerable<XmppTag> LookupTagsFromType(Type targetType)
        => s_ElementTypes.GetValueOrDefault(targetType) ?? s_Empty;

    static bool TryLookpSpecialElement(string tag, string? ns, [NotNullWhen(true)] out Type? result)
    {
        result = (tag, ns) switch
        {
            ("stream:stream", Namespaces.Stream) => typeof(StreamStream),
            ("stream:error", _) => typeof(StreamError),
            ("iq", _) => typeof(Iq),
            ("message", _) => typeof(Message),
            ("presence", _) => typeof(Presence),
            _ => null
        };

        return result != null;
    }

    public static Type? LookupType(string tagName, string? namespaceURI)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);

        if (TryLookpSpecialElement(tagName, namespaceURI, out var result))
            return result;

        var search = new XmppTag(tagName, namespaceURI);

        foreach (var (type, tags) in s_ElementTypes)
        {
            if (tags.Any(t => t == search))
                return type;
        }

        return null;
    }

    static Element? TryResolveElement(string tagName, string? namespaceURI, Element? context = default)
    {
        ElementFactoryResolver[] callbacks;

        lock (_callbacks)
            callbacks = _callbacks.ToArray();

        Element? result = default;

        foreach (var action in callbacks)
        {
            var temp = action(tagName, namespaceURI, context);

            if (temp != null)
            {
                result = temp;
                break;
            }
        }

        return result;
    }

    public static Element CreateElement(string tagName, string? namespaceURI, Element? context = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(tagName);

        Element? result = null;

        var type = LookupType(tagName, namespaceURI);

        if (type != null)
        {
            if (Activator.CreateInstance(type) is Element elem)
                result = elem;
        }

        if (result == null)
        {
            result = TryResolveElement(tagName, namespaceURI, context);

            if (result != null)
                return result;
        }

        if (result == null)
            result = new Element(tagName, namespaceURI);

        return result;
    }
}
