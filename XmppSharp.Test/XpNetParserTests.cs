using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using XmppSharp.Dom;
using XmppSharp.Parser;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Test;

[TestClass]
public class XpNetParserTests
{
	internal static Task<Element> ParseFromBuffer(string xml, [CallerMemberName] string callerName = default!)
		=> ParseFromBuffer(xml.GetBytes(), callerName);

	internal static async Task<Element> ParseFromBuffer(byte[] buf, [CallerMemberName] string callerName = default!)
	{
		using var ms = new MemoryStream(buf);
		ms.Position = 0;
		return await ParseFromStream(ms, callerName);
	}

	internal static async Task<Element> ParseFromStream(Stream stream, [CallerMemberName] string callerName = default!)
	{
		using var parser = new XmppStreamParser();

		var tcs = new TaskCompletionSource<Element>();

		bool isEndTag = false;

		parser.OnStreamStart += e =>
		{
			Console.WriteLine("ON STREAM START:\n{0}\n", e.ToString(true));
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		parser.OnStreamElement += e =>
		{
			Console.WriteLine("ON STREAM ELEMENT:\n{0}\n", e.ToString(true));

			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		parser.OnStreamEnd += () =>
		{
			isEndTag = true;
			tcs.TrySetResult(null!);
			return Task.CompletedTask;
		};

		_ = Task.Run(async () =>
		{
			await Task.Yield();

			try
			{
				// simulate IO
				int len;
				byte[] buf = new byte[4];

				while ((len = stream.Read(buf, 0, buf.Length)) > 0)
				{
					parser.Write(buf, len);
				}
			}
			catch (Exception ex)
			{
				tcs.TrySetException(ex);
			}
		});

		_ = Task.Delay(1500)
			.ContinueWith(_ => tcs.TrySetCanceled());

		var result = await tcs.Task;

		if (result == null && !isEndTag)
		{
			Assert.Fail("Unexpected null element.");
			return null;
		}

		return result!;
	}

	internal static void PrintResult(Element e)
	{
		Debug.WriteLine("\nRESULT:\n" + e.ToString(XmlFormatting.Indented) + "\n");
	}

	[TestMethod]
	public async Task ParseFromBuffer()
	{
		var elem = await ParseFromBuffer("<foo xmlns='bar' />");
		Debug.WriteLine("tagname= " + elem.TagName + ", uri= " + elem.GetNamespace());

		Assert.AreEqual("foo", elem.TagName);
		Assert.AreEqual("bar", elem.GetNamespace());
		PrintResult(elem);
	}

	[TestMethod]
	public async Task ParseElementWithComment()
	{
		var elem = await ParseFromBuffer("<foo><!--my comment--></foo>");
		Debug.WriteLine("ty= " + elem.FirstNode?.GetType()?.FullName ?? "<null>");

		if (elem.FirstNode is not Comment comment)
			Assert.Fail();
		else
			Assert.AreEqual("my comment", comment.Value);

		PrintResult(elem);
	}

	[TestMethod]
	public async Task ParseElementWithText()
	{
		var elem = await ParseFromBuffer("<foo>text</foo>");

		if (elem.FirstNode is not Text t)
			Assert.Fail("expected first node to be a Text node");
		else
			Assert.AreEqual(t.Value, "text");
	}

	[TestMethod]
	public async Task ParseElementWithCdata()
	{
		var elem = await ParseFromBuffer("<foo><![CDATA[my cdata]]></foo>");
		Debug.WriteLine("ty= " + elem.FirstNode?.GetType()?.FullName ?? "<null>");

		if (elem.FirstNode is not Cdata cdata)
			Assert.Fail();
		else
			Assert.AreEqual("my cdata", cdata.Value);

		PrintResult(elem);
	}

	[TestMethod]
	public async Task ParseWithCommentAndCdata()
	{
		var elem = await ParseFromBuffer("<foo><!--my comment--><![CDATA[my cdata]]></foo>");
		Debug.WriteLine("first node= " + elem.FirstNode?.GetType());
		Debug.WriteLine("last node= " + elem.LastNode?.GetType());

		var comment = elem.FirstNode as Comment;
		var cdata = elem.LastNode as Cdata;

		if (comment == null || cdata == null)
			Assert.Fail();

		Assert.AreEqual("my comment", comment.Value);
		Assert.AreEqual("my cdata", cdata.Value);

		PrintResult(elem);
	}

	[TestMethod]
	public async Task ParseOpenTag()
	{
		var streamId = Guid.NewGuid().ToString("N");

		var el = new StreamStream
		{
			Id = streamId,
			From = "localhost",
			Version = "1.0",
			Namespace = Namespaces.Client
		}.StartTag();

		var result = await ParseFromBuffer(el);

		Assert.IsNotNull(result);
		Console.WriteLine(result.GetType().FullName);
		Assert.AreEqual(streamId, result.GetAttribute("id"));
	}

	[TestMethod]
	public async Task ParseCloseTag()
	{
		var result = await ParseFromBuffer("</stream:stream>");
		Assert.IsNull(result);
	}

#if ENABLE_TCP_COMMS_TEST

	[TestMethod]
	public async Task ParseSampleJabberComms()
	{
		using var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
		socket.Bind(new IPEndPoint(IPAddress.Loopback, 6222));
		socket.Listen(1);

		using var client = await socket.AcceptAsync();
		using var stream = new NetworkStream(client, false);
		using var semaphore = new SemaphoreSlim(1, 1);

		var tcs = new TaskCompletionSource();

		var buf = new byte[4];
		int len;

		using var parser = new XmppStreamParser();
		bool isAuth = false;

		parser.OnStreamStart += async e =>
		{
			Console.WriteLine("recv <<:\n{0}\n", e.StartTag());

			await Task.Yield();
			e.Id = Environment.TickCount.ToString("x2");
			e.SwitchDirection();

			var features = new StreamFeatures
			{
				Mechanisms = new()
				{
					SupportedMechanisms = !isAuth ? [new("PLAIN")] : []
				},
			};

			if (isAuth)
			{
				await SendXml(e.EndTag());
				tcs.TrySetResult();
			}
			else
			{
				await SendXml(e.StartTag());
				await SendXml(features.ToString(true));
			}
		};

		parser.OnStreamElement += async e =>
		{
			Console.WriteLine("recv <<:\n{0}\n", e.ToString(true));

			await Task.Yield();

			if (e is Auth auth)
			{
				isAuth = true;
				await Send<Success>();
				parser.Reset();
			}
		};

		parser.OnStreamEnd += async () =>
		{
			Console.WriteLine("recv <<:\n{0}\n", Xml.XmppStreamEnd);
			await Task.Yield();
			tcs.TrySetResult();
		};

		_ = Task.Run(async () =>
		{
			try
			{
				while (true)
				{
					len = await stream.ReadAsync(buf);

					if (len == 0)
						break;

					parser.Write(buf, len);
				}
			}
			catch (IOException)
			{
				tcs.TrySetResult();
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		});

		await tcs.Task;

		Task Send<T>() where T : Element, new()
			=> SendXml(new T().ToString(true));

		async Task SendXml(string xml)
		{
			await semaphore.WaitAsync();

			try
			{
				await stream.WriteAsync(xml.GetBytes());
				Console.WriteLine("send >>:\n{0}\n", xml);
			}
			catch (IOException)
			{
				tcs.TrySetResult();
			}
			catch (Exception ex)
			{
				tcs.TrySetException(ex);
			}
			finally
			{
				semaphore.Release();
			}
		}
	}
#endif

	[TestMethod]
	public async Task ParseWithCommentAndCdataAndText()
	{
		var elem = await ParseFromBuffer("<foo><!--my comment-->SOME_TEXT_HERE<![CDATA[my cdata]]></foo>");
		Debug.WriteLine("first node= " + elem.FirstNode?.GetType());
		Debug.WriteLine("last node= " + elem.LastNode?.GetType());

		var comment = elem.FirstNode as Comment;
		var cdata = elem.LastNode as Cdata;

		if (comment == null || cdata == null)
			Assert.Fail();

		if (elem.Nodes().ElementAtOrDefault(1) is not Text t)
			Assert.Fail();
		else
		{
			Assert.IsNotNull(t.Value);
			Assert.AreEqual("SOME_TEXT_HERE", t.Value);
			Debug.WriteLine("middle node= " + t.GetType());
		}

		Assert.AreEqual("my comment", comment.Value);
		Assert.AreEqual("my cdata", cdata.Value);

		PrintResult(elem);
	}

	[TestMethod]
	public async Task ParseFromZipEntry()
	{
		using var fs = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "zipfile.zip"));
		using var archive = new ZipArchive(fs, ZipArchiveMode.Read);

		var entry = archive.GetEntry("snippet.xml");
		Assert.IsNotNull(entry);

		using var stream = entry.Open();
		var element = await ParseFromStream(stream);

		Assert.AreEqual("CodeSnippets", element.TagName);
		Assert.AreEqual("http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet", element.Namespace);
		Assert.AreEqual("CodeSnippet", element.FirstChild.TagName);
	}
}