#pragma warning disable CA2255

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Factory;

public static class ElementFactory
{
    [ModuleInitializer]
    internal static void Initialize()
        => RegisterElements(typeof(ElementFactory).Assembly);

    static readonly ConcurrentDictionary<string, Type> s_Registry = new();

    static string GetKey(string localName, string ns)
        => string.Concat('{', ns, '}', localName);

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

    public static void RegisterElement(Type type)
    {
        foreach (var tag in type.GetCustomAttributes<XmppTagAttribute>())
            s_Registry[GetKey(tag.LocalName, tag.Namespace)] = type;
    }

    public static void RegisterElement(string localName, string ns, Type type)
        => s_Registry[GetKey(localName, ns)] = type;

    public static Element Create(string localName, string prefix, string ns)
    {
        Element result;

        if (s_Registry.TryGetValue(GetKey(localName, ns), out var type))
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