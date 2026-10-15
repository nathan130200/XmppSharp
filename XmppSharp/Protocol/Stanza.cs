using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

public abstract class Stanza : IXmppSerializable
{
	public string? Id { get; set; }
	public Jid? From { get; set; }
	public Jid? To { get; set; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static readonly AsyncLocal<string?> s_DefaultNamespace = new();

	[NotNull]
	public static string? DefaultNamespace
	{
		get => s_DefaultNamespace.Value ?? Namespaces.Client;
		set => s_DefaultNamespace.Value = value;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal List<StanzaExtension>? _extensions;

	public void SwitchDirection()
	{
		(From, To) = (To, From);
	}

	public Stanza()
	{

	}

	public Stanza(Element element)
	{
		ArgumentNullException.ThrowIfNull(element);

		Id = element.GetAttribute("id");

		From = element.GetAttribute("from");

		To = element.GetAttribute("to");
	}

	public IEnumerable<StanzaExtension> GetExtensions()
	{
		lock (this)
		{
			if (_extensions is null) return [];

			return [.. _extensions];
		}
	}

	public IEnumerable<T> GetExtensions<T>() where T : StanzaExtension
	{
		List<T>? result = null;

		lock (this)
		{
			if (_extensions is null) return [];

			foreach (var ext in _extensions)
			{
				if (ext is T temp)
				{
					result ??= [];

					result.Add(temp);
				}
			}
		}
		return result ?? Enumerable.Empty<T>();
	}

	public T? GetExtension<T>() where T : StanzaExtension
	{
		lock (this)
		{
			if (_extensions is null) return null;

			foreach (var ext in _extensions)
			{
				if (ext is T result)
					return result;
			}
		}

		return null;
	}

	public void AddExtension(Element element)
	{
		_extensions ??= [];

		if (StanzaExtensionFactory.TryCreate(element, out var extension))
			_extensions.Add(extension);
		else
			_extensions.Add(new DynamicStanzaExtension(element));
	}

	public void AddExtension(StanzaExtension extension)
	{
		ArgumentNullException.ThrowIfNull(extension);

		lock (this)
		{
			_extensions ??= [];

			_extensions.Add(extension);
		}
	}

	public T AddExtension<T>() where T : StanzaExtension, new()
	{
		lock (this)
		{
			_extensions ??= [];

			if (GetExtension<T>() is not { } result)
			{
				result = new T();

				_extensions.Add(result);
			}

			return result;
		}
	}

	public void AddExtension<T>(Action<T> callback) where T : StanzaExtension, new()
	{
		ArgumentNullException.ThrowIfNull(callback);

		lock (this)
		{
			var se = new T();

			_extensions ??= [];

			_extensions.Add(se);

			callback(se);
		}
	}

	public void RemoveExtensions()
	{
		lock (this)
		{
			if (_extensions is null) return;

			_extensions.Clear();
		}
	}

	public abstract Element Serialize();
}
