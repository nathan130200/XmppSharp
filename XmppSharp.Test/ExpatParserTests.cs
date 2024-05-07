using System.IO.Compression;
using Expat;
using XmppSharp.Dom;
using XmppSharp.Parsers;
using XmppSharp.Protocol.Base;

namespace XmppSharp.Test;

[TestClass]
public class ExpatParserTests
{
	[TestMethod]
	public async Task ParseFromBuffer()
	{
		string xml = "<foo xmlns='bar'><baz/></foo>";

		using var parser = new ExpatXmppParser(ExpatEncodingType.Utf8);

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		parser.Write(xml.GetBytes(), true);

		var result = await tcs.Task;

		Console.WriteLine("XML:\n" + result.ToString(XmlFormatting.None));
	}

	[TestMethod]
	public async Task ParseStreamError()
	{
		// Not declared: xmlns:stream="http://etherx.jabber.org/streams"
		// but expat can parse even with missing namespace declaration 😛
		// test output will be:

		/*

		Element: stream:error
		Element: bad-namespace-prefix
		Attribute: xmlns=urn:ietf:params:xml:ns:xmpp-streams

		*/

		var xml = @"<stream:error>
<bad-namespace-prefix xmlns=""urn:ietf:params:xml:ns:xmpp-streams"" />
</stream:error>";

		using var parser = new ExpatXmppParser();

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		using var stream = new MemoryStream(xml.GetBytes());

		stream.Position = 0;

		_ = Task.Run(async () =>
		{
			// simulate IO

			try
			{
				var buf = new byte[16];
				int cnt;

				while (true)
				{
					cnt = await stream.ReadAsync(buf);
					parser.Write(buf, cnt, cnt == 0);

					if (cnt == 0)
						break;
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		});

		_ = Task.Delay(3000).ContinueWith(_ => tcs.TrySetCanceled());

		var element = await tcs.Task;

		Assert.IsNotNull(element);
		Assert.AreEqual("stream:error", element.TagName);

		Dump(element);
	}

	[TestMethod]
	public async Task ParseStreamStart()
	{
		var xml = new StreamStream
		{
			From = "localhost",
			To = "user1",
			Id = Guid.NewGuid().ToString(),
			Version = "1.0",
			Language = "en"
		}.StartTag();

		using var parser = new ExpatXmppParser();

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamStart += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		using var stream = new MemoryStream(xml.GetBytes());
		stream.Position = 0;

		_ = Task.Run(async () =>
		{
			// simulate IO

			try
			{
				var buf = new byte[16];
				int cnt;

				while (true)
				{
					cnt = await stream.ReadAsync(buf);
					parser.Write(buf, cnt, cnt == 0);

					if (cnt == 0)
						break;
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		});

		_ = Task.Delay(3000).ContinueWith(_ => tcs.TrySetCanceled());

		var element = await tcs.Task;

		Console.WriteLine("XML:\n" + element.ToString(XmlFormatting.Indented));
	}

	[TestMethod]
	public async Task ParseFromString()
	{
		// .NET XmlReader consider those whitespaces after XML Decl invalid and cannot parse, while expat just skip since it's still a well-formed XML.

		var xml = @"<?xml version=""1.0"" ?>

<GameItem name=""sniper_kneepads_01"" type=""armor"">
<mmo_stats>
	<param name=""item_category"" value=""""/>
	<param name=""price"" value=""""/>
	<param name=""shopcontent"" value=""0""/>
	<param name=""classes"" value=""S""/>
</mmo_stats>
<UI_stats>
	<param name=""icon"" value=""tbd_icon""/>
</UI_stats>
<slots>
	<slot main=""1"" name=""legarmor"">
		<materials>
			<Material default=""1"" display_name=""black"" icon=""mat_black"" name=""default"" surface_type=""mat_armor_legs""/>
		</materials>
		<assets>
			<asset mode=""tp"" name=""sniper_kneepads_01""/>
		</assets>
	</slot>
</slots>
<GameParams>
</GameParams>
</GameItem>
";

		using var parser = new ExpatXmppParser();

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		using var stream = new MemoryStream(xml.GetBytes());
		stream.Position = 0;

		_ = Task.Run(async () =>
		{
			// simulate IO

			try
			{
				var buf = new byte[16];
				int cnt;

				while (true)
				{
					cnt = await stream.ReadAsync(buf);
					parser.Write(buf, cnt, cnt == 0);

					if (cnt == 0)
						break;
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		});

		_ = Task.Delay(3000).ContinueWith(_ => tcs.TrySetCanceled());

		var element = await tcs.Task;

		Console.WriteLine("XML:\n" + element.ToString(XmlFormatting.Indented));
	}

	[TestMethod]
	public async Task ParseFromZipFile()
	{
		using var fs = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "zipfile.zip"));
		using var archive = new ZipArchive(fs, ZipArchiveMode.Read);

		var entry = archive.GetEntry("snippet.xml");
		Assert.IsNotNull(entry);

		using var parser = new ExpatXmppParser();

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		using var stream = entry.Open();

		_ = Task.Run(async () =>
		{
			// simulate IO

			var buf = new byte[entry.Length / 8];
			int cnt;

			while (true)
			{
				cnt = await stream.ReadAsync(buf);
				parser.Write(buf, cnt, cnt == 0);

				if (cnt == 0)
					break;
			}
		});

		_ = Task.Delay(3000).ContinueWith(_ => tcs.TrySetCanceled());

		var element = await tcs.Task;

		Assert.AreEqual("CodeSnippets", element.TagName);
		Assert.AreEqual("http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet", element.DefaultNamespace);
		Assert.AreEqual("CodeSnippet", element.FirstChild.TagName);

		Console.WriteLine("XML:\n" + element.ToString(XmlFormatting.None) + "\n");

		Dump(element);
	}

	[TestMethod]
	public async Task ParseRealSample()
	{
		var xml = @"<iq to=""dedicated@warface/PVE-64197"" id=""uid000000e4"" type=""result"" from=""masterserver@warface/pve_101"" xmlns=""jabber:client"">
  <query xmlns=""urn:cryonline:k01"">
    <setserver master_node=""192.168.1.80"" />
  </query>
</iq>";

		using var parser = new ExpatXmppParser();

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		using var stream = new MemoryStream(xml.GetBytes());
		stream.Position = 0;

		_ = Task.Run(async () =>
		{
			// simulate IO

			try
			{
				var buf = new byte[16];
				int cnt;

				while (true)
				{
					cnt = await stream.ReadAsync(buf);
					parser.Write(buf, cnt, cnt == 0);

					if (cnt == 0)
						break;
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		});

		_ = Task.Delay(3000).ContinueWith(_ => tcs.TrySetCanceled());

		var element = await tcs.Task;

		Console.WriteLine("XML:\n" + element.ToString(XmlFormatting.Indented));
	}

	static void Dump(Element e, int depth = 0)
	{
		var tab = new string(' ', depth);

		Console.Write(tab + "Element: " + e.TagName);

		tab = new string(' ', depth + 1);

		if (string.IsNullOrWhiteSpace(e.Value))
			Console.WriteLine();
		else
			Console.WriteLine(" (value: {0})", e.Value);

		foreach (var (key, value) in e.Attributes())
		{
			if (key == "xmlns " && value == e.Parent?.GetAttribute(key))
				continue;

			Console.WriteLine(tab + "Attribute: {0}={1}", key, value);
		}

		foreach (var child in e.Children())
			Dump(child, depth + 3);
	}
}
