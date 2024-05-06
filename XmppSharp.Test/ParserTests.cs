using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using XmppSharp.Dom;

namespace XmppSharp.Test;

[TestClass]
public class ParserTests
{
	internal static async Task<Element> ParseFromBuffer(string xml, [CallerMemberName] string callerName = default!)
	{
		using var stream = new MemoryStream();
		stream.Write(xml.GetBytes());
		stream.Position = 0;

		using var parser = new XmppParser(stream, bufferSize: 16);

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamStart += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		_ = Task.Run(async () =>
		{
			try
			{
				while (await parser)
					;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		});

		_ = Task.Delay(1500)
			.ContinueWith(_ => tcs.TrySetCanceled());

		return await tcs.Task;
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
	public async Task ParseWithInputStream()
	{
		using var ms = new MemoryStream();
		await ms.WriteAsync("<foo xmlns='bar'><baz/></foo>".GetBytes());
		ms.Position = 0;

		using var parser = new XmppParser(ms);

		Element el = default!;

		parser.OnStreamElement += e =>
		{
			el = e;
			return Task.CompletedTask;
		};

		while (el == null && await parser)
		{
			Console.WriteLine("parser::advance(): true");
			await Task.Delay(1);
		}

		Console.WriteLine("parser::advance(): false");
		Console.WriteLine(el);

		Console.WriteLine("XML:\n" + el!.ToString(XmlFormatting.Indented));
	}

	[TestMethod]
	public async Task ParseWithInputStreamAndCompetition()
	{
		using var ms = new MemoryStream();
		await ms.WriteAsync("<foo xmlns='bar'><baz/></foo>".GetBytes());
		ms.Position = 0;

		using var parser = new XmppParser(ms);

		var tcs = new TaskCompletionSource<Element>();

		parser.OnStreamElement += e =>
		{
			tcs.TrySetResult(e);
			return Task.CompletedTask;
		};

		_ = Task.Run(async () =>
		{
			while (true)
			{
				try
				{
					var result = await parser;
					Console.WriteLine("parser::advance(): " + result);
					await Task.Delay(1);

					if (!result)
						break;
				}
				catch (Exception e)
				{
					Console.WriteLine("ERROR: " + e);
				}
			}
		});

		_ = Task.Delay(1000).ContinueWith(_ => tcs.TrySetCanceled());

		var el = await tcs.Task;

		Console.WriteLine("XML:\n" + el!.ToString(XmlFormatting.Indented));
	}

	[TestMethod]
	public async Task ParseWithFactoryStream()
	{
		using var ms = new MemoryStream();
		await ms.WriteAsync("<foo xmlns='bar'><baz/></foo>".GetBytes());
		ms.Position = 0;

		using var parser = new XmppParser(() => ms);
		var el = await parser.GetNextElementAsync();

		Console.WriteLine("parser::advance(): false");
		Console.WriteLine(el);

		Console.WriteLine("XML:\n" + el!.ToString(XmlFormatting.Indented));
	}

	[TestMethod]
	public async Task ParseFromZipEntry()
	{
		using var fs = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "zipfile.zip"));
		using var archive = new ZipArchive(fs, ZipArchiveMode.Read);

		var entry = archive.GetEntry("snippet.xml");
		Assert.IsNotNull(entry);

		using var stream = entry.Open();

		using var parser = new XmppParser(stream);
		var element = await parser.GetNextElementAsync();

		Assert.AreEqual("CodeSnippets", element.TagName);
		Assert.AreEqual("http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet", element.DefaultNamespace);
		Assert.AreEqual("CodeSnippet", element.FirstChild.TagName);
	}
}