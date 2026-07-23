using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace XmppSharp.Protocol.Base;

public abstract class Stanza : DirectionalElement
{
	static readonly AsyncLocal<string?> s_DefaultNamespace = new();

	[NotNull, DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static string? DefaultNamespace
	{
		get => s_DefaultNamespace.Value ?? Namespaces.Client;
		set => s_DefaultNamespace.Value = value != null ? ValidateNamespace(value) : null;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	static readonly string[] s_ValidNames = ["iq", "message", "presence"];

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	static readonly string[] s_ValidNamespaces = [Namespaces.Client, Namespaces.Server, Namespaces.Component];

	static string ValidateName(string name)
	{
		if (!s_ValidNames.Contains(name))
			throw new ArgumentOutOfRangeException(nameof(name));

		return name;
	}

	internal static string ValidateNamespace(string ns)
	{
		if (!s_ValidNamespaces.Contains(ns))
			throw new ArgumentOutOfRangeException(nameof(ns));

		return ns;
	}

	internal Stanza(string name) : base(ValidateName(name), DefaultNamespace)
	{

	}

	public string? Id
	{
		get => GetAttribute("id");
		set => SetAttribute("id", value);
	}

	public Error? Error
	{
		get => Element<Error>();
		set
		{
			Element<Error>()?.Remove();

			AddChild(value);
		}
	}
}