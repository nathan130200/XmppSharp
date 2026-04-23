using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Xml;

/// <summary>
/// Provides factory methods for creating and registering element types based on tag names and namespaces.
/// </summary>
public static class ElementFactory
{
    static readonly Type BaseClassType = typeof(Element);

    class Entry(TagAttribute key, Type value)
    {
        public readonly TagAttribute Key = key;
        public readonly Type Value = value;
    }

    static readonly ConcurrentDictionary<Type, IEnumerable<TagAttribute>> s_ElementTypes = [];

    static ElementFactory()
    {
        RegisterTypes(typeof(ElementFactory).Assembly);
    }

    /// <summary>
    /// Registers all non-abstract classes that inherit from <see cref="Element"/> in the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for element types.</param>
    public static void RegisterTypes(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        foreach (var type in from t in assembly.GetTypes()
                             where t.IsClass && !t.IsAbstract
                             where t.IsSubclassOf(BaseClassType)
                             select t)
        {
            RegisterTypeCore(type, type.GetCustomAttributes<TagAttribute>());
        }
    }

    static void RegisterTypeCore(Type type, params IEnumerable<TagAttribute> tags)
    {
        s_ElementTypes[type] = tags;
    }

    /// <summary>
    /// Registers a single class that inherits from <see cref="Element"/>.
    /// </summary>
    /// <param name="type">The type to register.</param>
    /// <exception cref="ArgumentException">Thrown if the type is not a non-abstract class that inherits from <see cref="Element"/>.</exception>
    public static void RegisterType(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsClass || type.IsAbstract || !type.IsSubclassOf(BaseClassType))
        {
            throw new ArgumentException($"Type '{type}' must be a non-abstract class that inherits from '{BaseClassType}'.");
        }

        RegisterTypeCore(type, type.GetCustomAttributes<TagAttribute>());
    }

    /// <summary>
    /// Attempts to find a registered element type based on the provided tag name and namespace URI.
    /// </summary>
    /// <param name="tagName">The name of the XML tag.</param>
    /// <param name="namespaceURI">The namespace URI of the XML tag.</param>
    /// <param name="result">When this method returns, contains the type of the element if found; otherwise, null.</param>
    /// <returns>True if a matching element type is found; otherwise, false.</returns>
    public static bool TryFindElementType(string tagName, string? namespaceURI, [NotNullWhen(true)] out Type? result)
    {
        result = default;

        foreach (var (type, tags) in s_ElementTypes)
        {
            if (tags.Any(x => x.Name == tagName && x.NamespaceUri == namespaceURI))
            {
                result = type;
                break;
            }
        }

        return false;
    }

    /// <summary>
    /// Creates an instance of an element based on the provided tag name and namespace URI. If a registered type is found, it will be instantiated; otherwise, a generic <see cref="Element"/> will be created.
    /// </summary>
    /// <param name="tagName">The name of the XML tag.</param>
    /// <param name="namespaceURI">The namespace URI of the XML tag.</param>
    /// <returns>An instance of the element.</returns>
    public static Element Create(string tagName, string? namespaceURI)
    {
        if (TryFindElementType(tagName, namespaceURI, out Type? targetType))
        {
            try
            {
                var result = (Element)Activator.CreateInstance(targetType)!;
                result.TagName = tagName;
                result.NamespaceUri = namespaceURI;
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[ElementFactory] Uncaught exception while activating element type '{targetType}' -> {ex}");
            }
        }

        return new Element(tagName, namespaceURI);
    }
}

