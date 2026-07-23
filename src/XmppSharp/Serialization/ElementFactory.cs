using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Serialization;

public static class ElementFactory
{
	readonly struct ElementKey : IEquatable<ElementKey>
	{
		public readonly string Name;

		public readonly string Namespace;

		readonly int _hashCode;

		public ElementKey(string name, string ns, bool intern)
		{
			Name = intern ? string.Intern(name) : name;

			Namespace = intern ? string.Intern(ns) : ns;

			_hashCode = HashCode.Combine
			(
				string.GetHashCode(Name, StringComparison.Ordinal),
				string.GetHashCode(Namespace, StringComparison.Ordinal)
			);
		}

		public readonly override int GetHashCode() => _hashCode;

		public override bool Equals([NotNullWhen(true)] object? obj)
			=> obj is ElementKey other && Equals(other);

		public bool Equals(ElementKey other)
		{
			return string.Equals(Name, other.Name, StringComparison.Ordinal)
				&& Namespace.Equals(other.Namespace, StringComparison.Ordinal);
		}

		public static bool operator ==(ElementKey x, ElementKey y) => x.Equals(y);

		public static bool operator !=(ElementKey x, ElementKey y) => !x.Equals(y);
	}

	static readonly EqualityComparer<ElementKey> s_KeyComparer;

	static readonly ConcurrentDictionary<ElementKey, Delegate> s_ElementTypeMap;

	static ElementFactory()
	{
		s_KeyComparer = EqualityComparer<ElementKey>.Create
		(
			(x, y) => x.Equals(y),
			static x => x.GetHashCode()
		);

		s_ElementTypeMap = new(s_KeyComparer);

		RegisterAssembly(typeof(ElementFactory).Assembly);
	}

	public static void RegisterAssembly(Assembly assembly)
	{
		var types = from t in assembly.GetTypes()
					where t.IsClass && t.IsSubclassOf(typeof(Element))
					where t.GetCustomAttributes<TagAttribute>().Any()
					select t;

		RegisterTypes(types);
	}

	public static void RegisterType<T>() where T : Element, new()
		=> RegisterType(typeof(T));

	static void RegisterTypes(IEnumerable<Type> types)
	{
		foreach (var type in types)
			RegisterType(type);
	}

	static void RegisterType(Type type)
	{
		var attributes = type.GetCustomAttributes<TagAttribute>();

		if (!attributes.Any())
		{
			Trace.WriteLine($"ElementFactory::RegisterType(): Type '{type}' does not have any tag definition.");
			return;
		}

		var ctor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic, Type.EmptyTypes);

		if (ctor == null)
		{
			Trace.WriteLine($"ElementFactory::RegisterType(): Type '{type}' does not have an parameterless constructor.");
			return;
		}

		var fn = Expression.Lambda(Expression.New(ctor)).Compile();

		foreach (var attr in attributes)
			s_ElementTypeMap[new(attr.Name, attr.Namespace, true)] = fn;
	}

	public static Element CreateElement(string name, string? ns)
	{
		ns ??= string.Empty;

		var entry = new ElementKey(name, ns, false);

		if (s_ElementTypeMap.TryGetValue(entry, out var fn))
		{
			try
			{
				return (Element)fn.DynamicInvoke()!;
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"ElementFactory::CreateElement() Failed to construct element type '{fn}': {ex}");
			}
		}

		return new Element(name, ns);
	}
}
