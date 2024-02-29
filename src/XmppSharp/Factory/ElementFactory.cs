using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Factory;

public static class ElementFactory
{
#pragma warning disable

    [ModuleInitializer]
    internal static void InitializeCache()
    {
        try
        {
            RegisterAssembly(typeof(ElementFactory).Assembly);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"[ElementFactory::InitializeCache] Register built-in types failed: {e}");
        }
    }

#pragma warning restore

    readonly static Type s_BaseType = typeof(Element);
    readonly static ConcurrentDictionary<string, Type> s_registry = [];

    public static void RegisterAssembly(Assembly assembly)
    {
        var subTypes = assembly.GetTypes()
            .Where(t => t != s_BaseType && t.IsSubclassOf(s_BaseType) && !t.IsAbstract)
            .Where(t => t.GetCustomAttributes<XmppTagAttribute>().Any());

        foreach (var type in subTypes)
        {
            foreach (var attr in type.GetCustomAttributes<XmppTagAttribute>())
            {
                var key = BuildKey(attr.Name, attr.Namespace);
                s_registry[key] = type;
                Debug.WriteLine($"[ElementFactory::RegisterAssembly] assembly={assembly.GetName().Name}; Register type '{type}' with key '{key}'");
            }
        }
    }

    static string BuildKey(string name, string ns)
    {
        if (ns == null)
            return name;

        return string.Concat('{', ns, '}', name);
    }

    public static void AddElementType(Type type, string name, string xmlns = default)
    {
        var key = BuildKey(name, xmlns);
        s_registry[key] = type;
        Debug.WriteLine($"[ElementFactory::RegisterType] Register type '{type}' with key '{key}'");
    }

    public static Element CreateElement(string name, string ns = default)
    {
        var hasPrefix = name.ExtractQualifiedName(out var localName, out var prefix);

        var key = BuildKey(localName, ns);

        if (s_registry.TryGetValue(key, out var type))
            return Activator.CreateInstance(type) as Element;

        var el = new Element(name);

        if (hasPrefix)
            el.SetNamespace(prefix, ns);

        return el;
    }
}
