namespace XmppSharp;

//[TestClass]
//public class PositionalElementTest
//{
//    [TestMethod]
//    public void InsertBefore()
//    {
//        var element = new XmppElement("root");
//        var child = new XmppElement("foo", "bar");

//        var comment = new XmppComment("Hello World!");
//        element.AddChild(comment);

//        element.InsertBefore(child, comment);

//        Assert.AreEqual(element.FirstNode, child);
//    }

//    [TestMethod]
//    public void InsertAfter()
//    {
//        var element = new XmppElement("root");
//        var child = new XmppElement("foo", "bar");

//        var comment = new XmppComment("Hello World!");
//        element.AddChild(comment);

//        element.InsertAfter(child, comment);

//        Assert.AreEqual(element.LastNode, child);
//    }

//    [TestMethod]
//    public void ReplaceWith_NoParent_Throw()
//    {
//        var t = new XmppElement("tag");

//        Assert.ThrowsException<InvalidOperationException>(() =>
//        {
//            t.ReplaceWith(new XmppText("some text"));
//        });
//    }

//    [TestMethod]
//    public void ReplaceWith_NoThrow()
//    {
//        var t = new XmppElement("tag");

//        var e = t.C("child");
//        e.ReplaceWith(new XmppCdata("complex xml"));

//        Assert.IsInstanceOfType<XmppCdata>(t.FirstNode);
//    }
//}
