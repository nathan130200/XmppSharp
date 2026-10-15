using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using XmppSharp.Attributes;
using XmppSharp.Dom;
using XmppSharp.Protocol;

namespace XmppSharp.Serialization;

#pragma warning disable

public static class StanzaExtensionFactory
{
	internal sealed class StanzaExtensionInfo
	{
		public required Type ExtensionType { get; init; }

		public required Func<Element, BaseStanzaExtension> ElementConstructor { get; init; }

		public required bool IsCloneable { get; init; }
	}

	internal static readonly ConcurrentDictionary<XName, StanzaExtensionInfo> s_FactoryTypeMap = [];

	static StanzaExtensionFactory()
	{

	}

	[ModuleInitializer]
	internal static void Init()
	{
		RegisterExtensions(typeof(StanzaExtensionFactory).Assembly);
	}

	internal static bool CanReplaceExtensions
		= AppContext.TryGetSwitch("XmppSharp.StanzaExtensionFactory.AllowReplaceExistingExtensions", out var value) && value;

	public static void RegisterExtensions(Assembly assembly)
	{
		var baseType = typeof(BaseStanzaExtension);

		var entries = from type in assembly.GetTypes()
					  where type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType)
					  let attrs = type.GetCustomAttributes<StanzaExtensionAttribute>()
					  where attrs.Any()
					  select new
					  {
						  Type = type,
						  Keys = attrs.Select(x => XName.Get(x.LocalName, x.Namespace))
					  };

		if (!entries.Any())
			throw new InvalidOperationException($"Assembly '{assembly}' does not have any stanza extension implementation.");

		foreach (var entry in entries)
		{
			var elementConstructor = TryBindElementConstructor(entry.Type);

			var isCloneable = false;

			if (entry.Type.GetMethod("Clone") is MethodInfo cloneMethod)
				isCloneable = !(cloneMethod.IsAbstract || cloneMethod.DeclaringType != entry.Type);

			foreach (var key in entry.Keys)
			{
				bool isReplaced = false;

				if (s_FactoryTypeMap.TryGetValue(key, out var current))
				{
					if (!CanReplaceExtensions)
						throw new InvalidOperationException($"Stanza extension already registered for element '{key}' (implementation: '{current.ExtensionType}').");

					isReplaced = true;
				}

				s_FactoryTypeMap[key] = new()
				{
					ExtensionType = entry.Type,
					ElementConstructor = elementConstructor,
					IsCloneable = isCloneable
				};

				Trace.WriteLine($"[Stanza Extension Factory] {(isReplaced ? "Replaced" : "Registered")} implementation type '{entry.Type}' for element '{key}'.");
			}
		}
	}

	static readonly BindingFlags s_CtorMethodFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

	static readonly Type s_BaseType = typeof(Element);

	static readonly Type[] s_ElementCtorTypes = [typeof(Element)];

	static readonly ParameterExpression s_ElementParameter = Expression.Parameter(s_BaseType);

	static Func<Element, BaseStanzaExtension> TryBindElementConstructor(Type type)
	{
		var ctor = type.GetConstructor(s_CtorMethodFlags, s_ElementCtorTypes)
			?? throw new InvalidOperationException($"Cannot find element constructor for stanza extension type '{type}'.");

		var expr = Expression.Lambda<Func<Element, BaseStanzaExtension>>(
			Expression.New(ctor, s_ElementParameter),
				s_ElementParameter);

		return expr.Compile();
	}

	static bool TryGetExtensionInfo(ReadOnlySpan<char> localName, ReadOnlySpan<char> ns, [NotNullWhen(true)] out StanzaExtensionInfo? result)
	{
		result = null;

		foreach (var (key, value) in s_FactoryTypeMap)
		{
			if (localName.SequenceEqual(key.LocalName) && ns.SequenceEqual(key.NamespaceName))
			{
				result = value;
				return true;
			}
		}

		return false;
	}

	public static BaseStanzaExtension Create(Element element)
	{
		if (!TryGetExtensionInfo(element.LocalName, element.Namespace, out var entry))
			throw new InvalidOperationException($"Stanza extension for element '{element.LocalName} (uri: {element.Namespace}) is not registered.");

		return entry.ElementConstructor(element);
	}

	public static T Create<T>(Element element) where T : BaseStanzaExtension
	{
		if (!TryGetExtensionInfo(element.LocalName, element.Namespace, out var info))
			throw new InvalidOperationException($"Stanza extension for element '{element.LocalName} (uri: {element.Namespace}) is not registered.");

		if (info.ExtensionType != typeof(T))
			throw new InvalidOperationException("Stanza extension type mismatch.");

		return (T)info.ElementConstructor(element);
	}

	public static bool TryCreate(Element element, [NotNullWhen(true)] out BaseStanzaExtension? se)
	{
		se = null;

		if (TryGetExtensionInfo(element.LocalName, element.Namespace, out var info))
		{
			try
			{
				se = info.ElementConstructor(element);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		return se != null;
	}

	public static bool TryCreate<T>(Element element, [NotNullWhen(true)] out T? se) where T : BaseStanzaExtension
	{
		se = null;

		if (TryGetExtensionInfo(element.LocalName, element.Namespace, out var info))
		{
			if (info.ExtensionType != typeof(T))
				return false;

			try
			{
				se = (T)info.ElementConstructor(element);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		return se != null;
	}
}
