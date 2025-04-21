using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmppSharp.Dom;
using XmppSharp.Protocol;

namespace XmppSharp;

[TestClass]
public class DocumentParsingTests
{
    static readonly string SampleXml = @"<foo><bar xmlns='urn:mstest:app'><baz count='2' /></bar></foo>";

    [TestMethod]
    public void ParseFromString()
    {
        var doc = new XmppDocument();
        doc.Parse(SampleXml);

        Assert.IsNotNull(doc.RootElement);

        var root = doc.RootElement;
        Assert.AreEqual("foo", root.TagName);

        var child = root.FirstChild;
        Assert.IsNotNull(child);
        Assert.AreEqual("bar", child.TagName);
        Assert.AreEqual("urn:mstest:app", child.DefaultNamespace);

        var sub = child.FirstChild;
        Assert.IsNotNull(sub);
        Assert.AreEqual("2", sub.GetAttribute("count"));
    }

    [TestMethod]
    public void ParseFromFactory()
    {
        var xml = "<iq xmlns='jabber:client' type='result'><query xmlns='urn:cryonline:k01'><setserver master_node='localhost' /></query></iq>";

        var doc = new XmppDocument().Parse(xml);

        Assert.IsInstanceOfType<Iq>(doc.RootElement);

        var elem = doc.RootElement as Iq;
        Assert.IsNotNull(elem);

        Assert.AreEqual(IqType.Result, elem.Type);

        var query = elem.FirstChild;
        Assert.IsNotNull(query);

        Assert.AreEqual("query", query.TagName);
        Assert.AreEqual(Namespaces.CryOnline, query.Namespace);

        var child = query.FirstChild;
        Assert.IsNotNull(child);

        Assert.AreEqual("setserver", child.TagName);
        Assert.AreEqual(Namespaces.CryOnline, child.Namespace);
        Assert.AreEqual("localhost", child.GetAttribute("master_node"));
    }

    [TestMethod]
    public void ParseFromStream()
    {
        var doc = new XmppDocument();

        {
            var buffer = Encoding.UTF8.GetBytes(SampleXml);

            using (var stream = new MemoryStream(buffer))
                doc.Load(stream);
        }

        Assert.IsNotNull(doc.RootElement);

        var root = doc.RootElement;
        Assert.AreEqual("foo", root.TagName);

        var child = root.FirstChild;
        Assert.IsNotNull(child);
        Assert.AreEqual("bar", child.TagName);
        Assert.AreEqual("urn:mstest:app", child.DefaultNamespace);

        var sub = child.FirstChild;
        Assert.IsNotNull(sub);
        Assert.AreEqual("2", sub.GetAttribute("count"));
    }

    [TestMethod]
    public void ParseComment()
    {
        var xml = "<foo><!--this is a comment--></foo>";
        var doc = new XmppDocument().Parse(xml);

        var el = doc.RootElement;
        Assert.IsNotNull(el);

        Assert.AreEqual("foo", el.TagName);

        var child = el.FirstNode;
        Assert.IsInstanceOfType<XmppComment>(child);
        Assert.AreEqual("this is a comment", ((XmppComment)child).Value);
    }

    [TestMethod]
    public void ParseCdata()
    {
        var xml = "<foo><![CDATA[this is CDATA section <>]]></foo>";
        var doc = new XmppDocument().Parse(xml);

        var el = doc.RootElement;
        Assert.IsNotNull(el);

        Assert.AreEqual("foo", el.TagName);

        var child = el.FirstNode;
        Assert.IsInstanceOfType<XmppCdata>(child);
        Assert.AreEqual("this is CDATA section <>", ((XmppCdata)child).Value);

        Console.WriteLine(doc.ToString(true));
    }
}