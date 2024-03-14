#pragma warning disable CA2255

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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

    public readonly record struct Entry(string LocalName, string Namespace) : IEquatable<Entry>
    {
        public static Entry Get(string localName, string ns)
            => new(localName, ns);

        public override int GetHashCode()
        {
            if (Namespace == null)
                return LocalName.GetHashCode();

            return HashCode.Combine(LocalName, Namespace);
        }

        public bool Equals(Entry other)
        {
            return string.Equals(LocalName, other.LocalName, StringComparison.Ordinal)
                && string.Equals(Namespace, other.Namespace, StringComparison.Ordinal);
        }

        internal static readonly IEqualityComparer<Entry> s_Comparer = new EntryComparerImpl();

        class EntryComparerImpl : IEqualityComparer<Entry>
        {
            public bool Equals(Entry lhs, Entry rhs)
                => lhs.Equals(rhs);

            public int GetHashCode([DisallowNull] Entry obj)
                => obj.GetHashCode();
        }
    }

    static readonly ConcurrentDictionary<Entry, Type> s_Registry = new(Entry.s_Comparer);

    public static IEnumerable<Entry> OfType<T>()
        => s_Registry.Where(x => x.Value == typeof(T)).Select(x => x.Key);

    static Entry CreateEntry(string localName, string ns)
        => Entry.Get(localName, ns);

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
            s_Registry[CreateEntry(tag.LocalName, tag.Namespace)] = type;
    }

    public static void RegisterElement(string localName, string ns, Type type)
        => s_Registry[CreateEntry(localName, ns)] = type;

    public static Element Create(string localName, string prefix, string ns)
    {
        Element result;

        if (s_Registry.TryGetValue(CreateEntry(localName, ns), out var type))
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