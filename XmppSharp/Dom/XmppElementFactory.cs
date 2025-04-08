using System.Collections.Concurrent;
using System.Reflection;
using XmppSharp.Attributes;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Dom;

public delegate XmppElement? XmppElementFactoryResolver(string tagName, string? namespaceURI, XmppElement? parent);

public static class XmppElementFactory
{
    public static event XmppElementFactoryResolver OnResolveElement
    {
        add
        {
            Throw.IfNull(value);

            lock (_callbacks)
                _callbacks.Add(value);
        }
        remove
        {
            Throw.IfNull(value);

            lock (_callbacks)
                _callbacks.Remove(value);
        }
    }

    static readonly List<XmppElementFactoryResolver> _callbacks = new();
    static readonly ConcurrentDictionary<Type, IEnumerable<XmppTagAttribute>> s_ElementTypes = new();
    static readonly IEnumerable<XmppTagAttribute> s_Empty = Enumerable.Empty<XmppTagAttribute>();

    static XmppElementFactory()
    {
        RegisterAssembly(typeof(XmppElementFactory).Assembly);
    }

    public static void RegisterType<T>()
        => RegisterType(typeof(T));

    public static void RegisterType(Type type)
    {
        Throw.IfNull(type);

        var tags = type.GetCustomAttributes<XmppTagAttribute>();

        if (!tags.Any())
            return;

        if (!s_ElementTypes.TryGetValue(type, out var current))
            s_ElementTypes[type] = tags;
        else
            s_ElementTypes[type] = current.Concat(tags);
    }

    public static void RegisterAssembly(Assembly assembly)
    {
        Throw.IfNull(assembly);

        var elements = from type in assembly.GetTypes()
                       where !type.IsAbstract && type.IsSubclassOf(typeof(XmppElement))
                       let tags = type.GetCustomAttributes<XmppTagAttribute>()
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

    public static IEnumerable<XmppTagAttribute> LookupTagsFromType<T>() => LookupTagsFromType(typeof(T));

    public static IEnumerable<XmppTagAttribute> LookupTagsFromType(Type targetType)
        => s_ElementTypes.GetValueOrDefault(targetType) ?? s_Empty;

    static readonly Dictionary<string, Type> s_TagNameToType = new()
    {
        [("stream:stream")] = typeof(StreamStream),
        [("stream:error")] = typeof(StreamError),
        [("stream:features")] = typeof(StreamFeatures),
        [("iq")] = typeof(Iq),
        [("message")] = typeof(Message),
        [("presence")] = typeof(Presence),
    };

    public static Type? ResolveType(string tagName, string? namespaceURI)
    {
        Throw.IfNullOrWhiteSpace(tagName);

        foreach (var (type, tags) in s_ElementTypes)
        {
            if (tags.Any(t => IsTagMatch(t, tagName, namespaceURI)))
                return type;
        }

        if (s_TagNameToType.TryGetValue(tagName, out var result))
            return result;

        return null;
    }

    static bool IsTagMatch(XmppTagAttribute attr, string tagName, string? ns)
    {
        return string.Equals(attr.TagName, tagName, StringComparison.Ordinal)
            && string.Equals(attr.NamespaceURI, ns, StringComparison.Ordinal);
    }

    static XmppElement? TryResolveElement(string tagName, string? namespaceURI, XmppElement? context = default)
    {
        XmppElementFactoryResolver[] callbacks;

        lock (_callbacks)
            callbacks = _callbacks.ToArray();

        XmppElement? result = default;

        foreach (var action in callbacks)
        {
            var entry = action(tagName, namespaceURI, context);

            if (entry != null)
            {
                result = entry;
                break;
            }
        }

        return result;
    }

    public static XmppElement Create(string tagName, string? namespaceURI, XmppElement? context = default)
    {
        Throw.IfNullOrWhiteSpace(tagName);

        var type = ResolveType(tagName, namespaceURI);

        if (type is not null)
        {
            if (Activator.CreateInstance(type) is XmppElement self)
                return self;
        }

        var result = TryResolveElement(tagName, namespaceURI, context);

        if (result != null)
            return result;

        if (result == null)
            result = new XmppElement(tagName, namespaceURI);

        return result;
    }
}
