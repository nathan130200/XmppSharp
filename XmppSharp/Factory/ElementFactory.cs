using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using XmppSharp.Attributes;

namespace XmppSharp.Factory;

/// <summary>
/// Provides a central mechanism for registering and creating XML elements in a type-safe manner,
/// primarily used within the XMPP context.
/// </summary>
public static class ElementFactory
{
    [ModuleInitializer]
    internal static void Initialize()
        => RegisterElements(typeof(ElementFactory).Assembly);

    static readonly ConcurrentDictionary<XmlTagInfo, Type> ElementTypes = new(XmlTagInfo.Comparer);

    public static IEnumerable<XmlTagInfo> GetTags<T>() where T : Element
        => ElementTypes.Where(x => x.Value == typeof(T)).Select(x => x.Key);

    public static void RegisterElement<T>() where T : Element
        => RegisterElement(typeof(T));

    public static void RegisterElements(Assembly assembly)
    {
        var types = from type in assembly.GetTypes()
                    where !type.IsAbstract && type.IsSubclassOf(typeof(Element))
                        && type.GetCustomAttributes<XmppTagAttribute>().Any()
                    select type;

        foreach (var type in types)
            RegisterElement(type);
    }

    public static void RegisterElement(Type type)
    {
        foreach (var tag in type.GetCustomAttributes<XmppTagAttribute>())
            ElementTypes[new(tag.LocalName, tag.NamespaceURI)] = type;
    }

    public static void RegisterElement(string localName, string ns, Type type)
        => ElementTypes[new(localName, ns)] = type;

    static bool GetElementType(string localName, string ns, out Type type)
    {
        type = default;

        foreach (var (tag, value) in ElementTypes)
        {
            if (tag.LocalName == localName && tag.NamespaceURI == ns)
            {
                type = value;
                break;
            }
        }

        return type != null;
    }

    public static Element Create(string qualifiedName, string ns)
    {
        Element elem = default;

        if (GetElementType(qualifiedName, ns, out var type))
            elem = Activator.CreateInstance(type) as Element;
        else
            elem = new Element(qualifiedName);

        if (!string.IsNullOrEmpty(elem.Prefix))
            elem.SetNamespace(elem.Prefix, ns);
        else
            elem.SetNamespace(ns);

        return elem;
    }
}