using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp.Dom;

public static class XmppElementFactory
{
	static readonly ConcurrentDictionary<string, Type> s_TypeMap = new(StringComparer.Ordinal);

	static XmppElementFactory()
	{
		RegisterElementsFromAssembly(typeof(XmppElementFactory).Assembly);
	}

	static string BuildKey(string name, string? @namespace)
		=> string.Concat('{', @namespace ?? string.Empty, '}', name);

	public static void RegisterElementsFromAssembly(Assembly assembly)
	{
		var types = from type in assembly.GetExportedTypes()

					where type.IsClass
						&& type.IsSubclassOf(typeof(XmppElement))
						&& !type.IsAbstract

					select type;

		foreach (var type in types)
			RegisterTypeInternal(type);
	}

	public static void RegisterType(Type type)
	{
		if (!type.IsSubclassOf(typeof(XmppElement)))
			throw new ArgumentException("Element type is not an subclass of XmppElement.", nameof(type));

		if (type.IsAbstract)
			throw new ArgumentException("Element type cannot be abstract.", nameof(type));

		RegisterTypeInternal(type);
	}

	static void RegisterTypeInternal(Type type)
	{
		foreach (var attr in type.GetCustomAttributes<TagAttribute>())
		{
			var key = BuildKey(attr.Name, attr.Namespace);

			if (s_TypeMap.TryAdd(key, type))
				Debug.WriteLine($"XmppElementFactory::RegisterType(): Register element type '{type}' with key '{key}'.");
			else
			{
				s_TypeMap[key] = type;
				Debug.WriteLine($"XmppElementFactory::RegisterType(): Overwrite element key '{key}' with new type '{type}'.");
			}
		}
	}

	public static XmppElement CreateElement(string tagName, string? uri)
	{
		var key = BuildKey(tagName, uri);

		if (s_TypeMap.TryGetValue(key, out var type))
		{
			Debug.WriteLine($"XmppElementFactory::CreateElement(): Found element type with key '{key}' (type: {type})...");

			try
			{
				if (Activator.CreateInstance(type) is XmppElement element)
				{
					Debug.WriteLine($"XmppElementFactory::CreateElement(): Construct typed element '{type}' (namespace: '{uri}')");
					return element;
				}
			}
			catch (Exception ex)
			{
				Trace.TraceError($"XmppElementFactory::CreateElement(): Failed to construct typed element '{type}': {ex}\n");
			}
		}


		Debug.WriteLine($"XmppElementFactory::CreateElement(): Construct fallback element with name '{tagName}' (namespace: '{uri}')");

		return new XmppElement(tagName, uri);
	}
}