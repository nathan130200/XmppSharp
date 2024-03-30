using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Factory;

public static class ElementFactory
{
    [ModuleInitializer]
    internal static void InitStatic()
        => RegisterElements(typeof(ElementFactory).Assembly);

    static readonly ConcurrentDictionary<XmlTagName, Type> ElementTypes
        = new(XmlTagName.Comparer);

    public static IEnumerable<XmlTagName> GetTags<T>()
        => ElementTypes.Where(x => x.Value == typeof(T)).Select(x => x.Key);

    public static void RegisterElement<T>()
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

    // FIXME: i don't have a better idea to cache this, instead just allocate new and try lookup.
    static XmlTagName Get(string localName, string ns)
        => new(localName, ns);

    public static void RegisterElement(Type type)
    {
        foreach (var tag in type.GetCustomAttributes<XmppTagAttribute>())
            ElementTypes[Get(tag.LocalName, tag.Namespace)] = type;
    }

    public static void RegisterElement(string localName, string ns, Type type)
        => ElementTypes[Get(localName, ns)] = type;

    static bool GetElementType(string localName, string ns, out Type type)
    {
        type = default;

        foreach (var (tag, value) in ElementTypes)
        {
            if (tag.LocalName == localName
                && tag.Namespace == ns)
            {
                type = value;
                break;
            }
        }

        return type != null;
    }

    public static Element Create(string localName, string prefix, string ns)
    {
        Element result;

        if (GetElementType(localName, ns, out var type))
            result = Activator.CreateInstance(type) as Element;
        else
        {
            result = new Element(localName)
            {
                Prefix = prefix
            };
        }

        if (!string.IsNullOrEmpty(prefix))
            result.SetNamespace(prefix, ns);
        else
            result.SetNamespace(ns);

        return result;
    }
}