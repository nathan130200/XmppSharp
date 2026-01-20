using System.Text;
using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Base;

[Tag("stream", Namespaces.Stream)]
[Tag("stream:stream", Namespaces.Stream)]
public class Stream : DirectionalElement
{
	public Stream() : base("stream:stream", Namespaces.Stream)
	{

	}

	public string? Id
	{
		get => GetAttribute("id");
		set => SetAttribute("id", value);
	}

	public string? Version
	{
		get => GetAttribute("version");
		set => SetAttribute("version", value);
	}

	public string? Language
	{
		get => GetAttribute("xml:lang");
		set => SetAttribute("xml:lang", value);
	}

	public void GenerateId()
		=> Id = Guid.NewGuid().ToString("d");

	public string StartTag()
	{
		var sb = new StringBuilder();

		lock (this)
		{
			using (var writer = CreateXmlWriter(sb, false))
				WriteStartElement(writer);
		}

		return sb.ToString();
	}
}
