using XmppSharp.Dom;

namespace XmppSharp;

[TestClass]
public class PositionalElementTest
{
    [TestMethod]
    public void InsertBefore()
    {
        var element = new Element("root");
        var child = new Element("foo", "bar");

        var comment = new Comment("Hello World!");
        element.AddChild(comment);

        element.InsertBefore(child, comment);

        Assert.AreEqual(element.FirstNode, child);
    }

    [TestMethod]
    public void InsertAfter()
    {
        var element = new Element("root");
        var child = new Element("foo", "bar");

        var comment = new Comment("Hello World!");
        element.AddChild(comment);

        element.InsertAfter(child, comment);

        Assert.AreEqual(element.LastNode, child);
    }

    [TestMethod]
    public void ReplaceWith_NoParent_Throw()
    {
        var t = new Element("tag");

        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            t.ReplaceWith(new Text("some text"));
        });
    }

    [TestMethod]
    public void ReplaceWith_NoThrow()
    {
        var t = new Element("tag");

        var e = t.C("child");
        e.ReplaceWith(new Cdata("complex xml"));

        Assert.IsInstanceOfType<Cdata>(t.FirstNode);
    }
}
