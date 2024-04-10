using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
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

    static readonly ConcurrentDictionary<XName, Type> ElementTypes
        = new(XNameComparer.Shared);

    class XNameComparer : IEqualityComparer<XName>
    {
        public static readonly XNameComparer Shared = new();

        public bool Equals(XName? x, XName? y)
        {
            return x.LocalName.Equals(y.LocalName, StringComparison.Ordinal)
                && x.NamespaceName.Equals(y.NamespaceName, StringComparison.Ordinal);
        }

        public int GetHashCode(XName obj)
            => obj.GetHashCode();
    }

    /// <summary>
    /// Retrieves a collection of registered tag names for a specific element type.
    /// <para>This method can be useful for introspection purposes to discover the registered tag names associated with a particular element class.</para>
    /// </summary>
    /// <typeparam name="T">The type of element for which to retrieve registered tag names.</typeparam>
    /// <returns>An enumerable collection of <see cref="XName"/> instances representing the registered tag names for the specified element type.</returns>
    public static IEnumerable<XName> GetTags<T>()
        => ElementTypes.Where(x => x.Value == typeof(T)).Select(x => x.Key);

    /// <summary>
    /// Registers an element type for future creation using the element type itself.
    /// <para>This method is typically used internally during automatic registration or explicitly to associate a specific element class with its corresponding XML tag.</para>
    /// </summary>
    /// <typeparam name="T">The type of element to register.</typeparam>
    public static void RegisterElement<T>()
        => RegisterElement(typeof(T));

    /// <summary>
    /// Registers element types found within a specified assembly.
    /// <para>
    /// This method scans the provided assembly for classes inheriting from <see cref="XElement"/> and decorated with the
    /// <see cref="XmppTagAttribute"/> attribute.
    /// </para>
    /// <para>
    /// For each identified element class,
    /// it registers the class type with the corresponding tag name extracted from the attribute.
    /// </para>
    /// </summary>
    /// <param name="assembly">The assembly to scan for element types.</param>
    public static void RegisterElements(Assembly assembly)
    {
        var types = from type in assembly.GetTypes()
                    where !type.IsAbstract && type.IsSubclassOf(typeof(XElement))
                        && type.GetCustomAttributes<XmppTagAttribute>().Any()
                    select type;

        foreach (var type in types)
            RegisterElement(type);
    }

    /// <summary>
    /// Registers an element type for future creation based on its XML tag name.
    /// <para>This method extracts tag information from a <see cref="XmppTagAttribute"/> attribute applied to the type.</para>
    /// </summary>
    /// <param name="type">The element type to register.</param>
    public static void RegisterElement(Type type)
    {
        foreach (var tag in type.GetCustomAttributes<XmppTagAttribute>())
            ElementTypes[tag.Name] = type;
    }

    /// <summary>
    /// Registers an element type for future creation using a specific XML tag name.
    /// <para>This method provides direct association between a tag name and an element type without relying on a dedicated attribute.</para>
    /// </summary>
    /// <param name="localName">The local name of the XML tag.</param>
    /// <param name="ns">The namespace URI of the XML tag.</param>
    /// <param name="type">The element type to register.</param>
    public static void RegisterElement(string localName, string ns, Type type)
        => ElementTypes[XName.Get(localName, ns)] = type;

    static bool GetElementType(string localName, string ns, out Type type)
    {
        type = default;

        foreach (var (tag, value) in ElementTypes)
        {
            if (tag.LocalName == localName && tag.Namespace == ns)
            {
                type = value;
                break;
            }
        }

        return type != null;
    }

    /// <summary>
    /// Creates a new XML element instance based on the provided local name, prefix, and namespace.
    /// <para>This method first attempts to locate a registered element type matching the specified tag name (local name and namespace).</para>
    /// <para>If a registered type is found, it creates an instance of that type using reflection.</para>
    /// <para>Otherwise, it creates a generic <see cref="XElement"/> instance with the provided local name and sets the appropriate namespace using the prefix or directly.</para>
    /// </summary>
    /// <param name="localName">The local name of the element.</param>
    /// <param name="prefix">The namespace prefix for the element (optional).</param>
    /// <param name="ns">The namespace URI of the element.</param>
    /// <returns>A new XML element instance of the registered type or a generic <see cref="XElement"/> instance if no matching type is found.</returns>
    public static XElement Create(string localName, string prefix, string ns)
    {
        XElement result;

        if (GetElementType(localName, ns, out var type))
            result = Activator.CreateInstance(type) as XElement;
        else
        {
            XName name = ns != null ? XName.Get(localName, ns) : localName;

            result = new XElement(name);

            if (!string.IsNullOrEmpty(prefix))
                result.Add(new XAttribute(XNamespace.Xmlns + prefix, ns));
        }

        return result;
    }
}