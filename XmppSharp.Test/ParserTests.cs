using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using XmppSharp.Dom;

namespace XmppSharp.Test;

[TestClass]
public class ParserTests
{
    internal static async Task<Element> ParseFromBuffer([StringSyntax("Xml")] string xml, [CallerMemberName] string callerName = default!)
    {
        using var stream = new MemoryStream();
        stream.Write(xml.GetBytes());
        stream.Position = 0;

        using var parser = new XmppParser(bufferSize: 16);
        parser.Reset(stream);

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
                while (true)
                {
                    await Task.Delay(1);

                    var result = await parser.Advance();

                    if (!result)
                        break;
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine($"<ERROR>\n{error}\n");
                tcs.TrySetException(error);
            }
        });

        Exception timeout;

        try
        {
            throw new OperationCanceledException("Parser took too long?");
        }
        catch (Exception e)
        {
            timeout = e;
        }

        _ = Task.Delay(5000)
            .ContinueWith(_ => tcs.TrySetException(timeout));

        return await tcs.Task;
    }

    internal static void PrintResult(Element e)
    {
        Debug.WriteLine("\n\nRESULT:\n\n" + e.ToString(true) + "\n");
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
}