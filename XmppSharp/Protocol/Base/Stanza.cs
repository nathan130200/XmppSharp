using System.Diagnostics;
using XmppSharp.Protocol.Extensions.XEP0203;

namespace XmppSharp.Protocol.Base;

public abstract class Stanza : DirectionalElement
{
	protected Stanza(string tagName, string? namespaceURI = null, object? value = null) : base(tagName, namespaceURI, value)
	{
	}

	public string? Id
	{
		get => GetAttribute("id");
		set => SetAttribute("id", value);
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public string? Type
	{
		get => GetAttribute("type");
		set => SetAttribute("type", value);
	}

	public string? Language
	{
		get => GetAttribute("xml:lang");
		set => SetAttribute("xml:lang", value);
	}

	public void GenerateId()
		=> Id = Guid.NewGuid().ToString("d");

	public StanzaError? Error
	{
		get => Element<StanzaError>();
		set
		{
			Element<StanzaError>()?.Remove();
			AddChild(value);
		}
	}

	public Delay? Delay
	{
		get => Element<Delay>();
		set
		{
			Element<Delay>()?.Remove();
			AddChild(value);
		}
	}
}
