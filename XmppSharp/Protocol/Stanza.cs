using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using XmppSharp.Dom;
using XmppSharp.Serialization;

namespace XmppSharp.Protocol;

/// <summary>
/// Base class for XMPP stanza abstractions.
/// </summary>
public abstract class Stanza : IXmppSerializable
{
	/// <summary>
	/// The ID of the stanza.
	/// </summary>
	public string? Id { get; set; }

	/// <summary>
	/// The origin of the stanza.
	/// </summary>
	public Jid? From { get; set; }

	/// <summary>
	/// The stanza destination.
	/// </summary>
	public Jid? To { get; set; }

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal static readonly AsyncLocal<string?> s_DefaultNamespace = new();

	/// <summary>
	/// Default namespace of stanza used in serializer.
	/// </summary>
	[NotNull]
	public static string? DefaultNamespace
	{
		get => s_DefaultNamespace.Value ?? Namespaces.Client;
		set => s_DefaultNamespace.Value = value;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	internal List<BaseStanzaExtension>? _extensions;

	/// <summary>
	/// Swap values between <see cref="From"/> and <see cref="To"/>.
	/// </summary>
	public void SwitchDirection()
	{
		(From, To) = (To, From);
	}

	/// <summary>
	/// Parameterless constructor.
	/// </summary>
	public Stanza()
	{

	}

	/// <summary>
	/// Element constructor, used to parse existing element.
	/// </summary>
	/// <param name="element">Element to extract stanza components.</param>
	internal Stanza(Element element)
	{
		ArgumentNullException.ThrowIfNull(element);

		Id = element.GetAttribute("id");

		From = element.GetAttribute("from");

		To = element.GetAttribute("to");
	}

	/// <summary>
	/// Returns the list of the stanza extensions.
	/// </summary>
	public IEnumerable<BaseStanzaExtension> GetExtensions()
	{
		lock (this)
		{
			if (_extensions is null) return [];

			return [.. _extensions];
		}
	}

	/// <summary>
	/// Find all stanza extensions with given type.
	/// </summary>
	/// <typeparam name="T">The extension type to look for.</typeparam>
	public IEnumerable<T> GetExtensions<T>() where T : BaseStanzaExtension
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

	/// <summary>
	/// Find first stanza extensions of given type.
	/// </summary>
	/// <typeparam name="T">The extension type to look for.</typeparam>
	public T? GetExtension<T>() where T : BaseStanzaExtension
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

	/// <summary>
	/// Add stanza extension from element.
	/// </summary>
	/// <param name="element">The element to parse.</param>
	public void AddExtension(Element element)
	{
		_extensions ??= [];

		if (StanzaExtensionFactory.TryCreate(element, out var extension))
			_extensions.Add(extension);
		else
			_extensions.Add(new StanzaExtension(element));
	}

	/// <summary>
	/// Add stanza extension.
	/// </summary>
	/// <param name="extension">The extendion to add.</param>
	public void AddExtension(BaseStanzaExtension extension)
	{
		ArgumentNullException.ThrowIfNull(extension);

		lock (this)
		{
			_extensions ??= [];

			_extensions.Add(extension);
		}
	}

	/// <summary>
	/// Create and add stanza extension to this stanza.
	/// </summary>
	/// <typeparam name="T">The extension type to construct.</typeparam>
	/// <returns>Stanza extension type of <typeparamref name="T"/>.</returns>
	public T AddExtension<T>() where T : BaseStanzaExtension, new()
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

	/// <summary>
	/// Create and add stanza extension to this stanza.
	/// </summary>
	/// <param name="callback">Callback to mutate the stanza extension instance.</param>
	/// <typeparam name="T">The extension type to construct.</typeparam>
	public void AddExtension<T>(Action<T> callback) where T : BaseStanzaExtension, new()
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

	/// <summary>
	/// Remove all stanza extensions.
	/// </summary>
	public void RemoveAllExtensions()
	{
		lock (this)
		{
			if (_extensions is null) return;

			_extensions.Clear();
		}
	}

	/// <summary>
	/// Abstract method to construct XML element of this stanza.
	/// </summary>
	public abstract Element Serialize();
}
