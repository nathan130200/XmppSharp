using XmppSharp.Dom;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Core;
using XmppSharp.Protocol.Extensions.XEP0114;
using Xunit.Abstractions;

namespace XmppSharp.Test;

public class StanzaTests(ITestOutputHelper Output)
{
	[Fact]
	public void TestDynamicStanzaExtension()
	{
		var stz = new Iq(IqType.Result)
		{
			Id = $"uid{3:x8}",
			From = "k01.warface.servers",
			To = "dedicated@warface/GameClient"
		};

		{
			var query = new Element("query", Namespaces.CryOnline);

			query.C("setserver", x =>
			{
				x.SetAttribute("master_node", "localhost");
			});

			stz.AddExtension(new StanzaExtension(query));
		}

		var result = stz.Serialize();

		var attr = result["query", Namespaces.CryOnline]?["setserver"]?.Attribute("master_node");

		Assert.NotNull(attr);

		Assert.Equal("localhost", attr.Value);

		Output.WriteLine("XML: " + result.ToString(true));
	}

	[Fact]
	public void TestForComponentHandshake()
	{
		var streamId = "1234";

		var secret = "youshallnotpass";

		var token = Handshake.CreateAuthenticationToken(streamId, secret);

		Assert.Equal("fec9cff708f704a7f92e226c63825a4b54bc85f6", token);
	}

	[Fact]
	public void CreateStanza()
	{
		var stz = new Iq(IqType.Result)
		{
			To = "usuario1@localhost.im"
		};

		var bind = stz.AddExtension<Bind>();

		bind.Jid = "usuario1@localhost.im/quarto";

		var element = stz.Serialize();

		var query = element.Element("bind", Namespaces.Bind);

		Assert.NotNull(query);

		var str = query.GetTag("jid");

		Assert.NotNull(str);

		var jid = new Jid(str);

		Assert.Equal("usuario1", jid.User);

		Assert.Equal("localhost.im", jid.Server);

		Assert.Equal("quarto", jid.Resource);

		Output.WriteLine("XML: " + element);
	}

	[Fact]
	public void CreateMultiCultureMessage()
	{
		var msg = new Message(MessageType.GroupChat);
		msg.SetBody("pt-BR", "Olá Mundo!");
		msg.SetBody(string.Empty, "Hello World!");

		var xml = msg.Serialize();

		Assert.Equal("groupchat", xml.Attribute("type"));

		var bodies = xml.Elements("body");

		var bodyWithoutLang = bodies.First(x => !x.HasAttribute("xml:lang"));

		Assert.Equal("Hello World!", bodyWithoutLang.Value);

		var bodyWithLang = bodies.First(x => x.HasAttribute("xml:lang"));

		Assert.Equal("Olá Mundo!", bodyWithLang.Value);

		Output.WriteLine("XML: " + xml.ToString(true));
	}

	[Fact]
	public void ParseMultiCultureMessage()
	{
		var xml = new Element("message", Namespaces.Client)
			.C("body", x =>
			{
				x.SetAttribute("xml:lang", "pt-BR");
				x.Value = "Olá Mundo!";
			})
			.C("body", x =>
			{
				x.Value = "Hello World!";
			})
			.C("subject", x =>
			{
				x.Value = "Message For You!";
			})
			.C("subject", x =>
			{
				x.SetAttribute("xml:lang", "pt-BR");
				x.Value = "Mensagem Para Você!";
			});

		var msg = new Message(xml);

		Assert.Equal("Hello World!", msg.Body);

		Assert.Equal("Olá Mundo!", msg.GetBody("pt-BR"));

		Assert.Equal("Message For You!", msg.Subject);

		Assert.Equal("Mensagem Para Você!", msg.GetSubject("pt-BR"));

		Output.WriteLine("XML: " + msg.Serialize().ToString(true));
	}
}
