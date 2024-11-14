using XmppSharp.Collections;

namespace XmppSharp;

[TestClass]
public class JidTests
{
    [TestMethod]
    [DataRow("foo.bar")]
    [DataRow("foo@bar")]
    [DataRow("foo@bar/baz")]
    [DataRow("foo@/baz")]
    [DataRow("/baz")]
    [DataRow("/")]
    public void ParseJid(string str)
    {
        Jid j = str!;
        Assert.AreEqual(str, j.ToString());
    }

    [TestMethod]
    public void ShouldBeEqualsBare()
    {
        var j1 = new Jid("foo@bar");
        var j2 = new Jid("foo@bar");
        Assert.AreEqual(0, BareJidComparer.Shared.Compare(j1, j2));
    }

    [TestMethod]
    public void ShouldBeEqualsFull()
    {
        var j1 = new Jid("foo@bar/baz");
        var j2 = new Jid("foo@bar/baz");

        // ensure not same instance
        Assert.IsFalse(ReferenceEquals(j1, j2));

        // ensure == 0
        Assert.AreEqual(0, FullJidComparer.Shared.Compare(j1, j2));
    }

    [TestMethod]
    public void ShouldNotBeEqualsBecauseResourceIsAbsent()
    {
        var j1 = new Jid("foo@bar");
        var j2 = new Jid("foo@bar/baz");

        Assert.IsFalse(ReferenceEquals(j1, j2));
        Assert.AreEqual(-1, FullJidComparer.Shared.Compare(j1, j2));
    }

    [TestMethod]
    public void ShouldBeEqualsEvenResourceIsNull()
    {
        var j1 = new Jid("foo@bar");
        var j2 = new Jid("foo@bar");

        Assert.IsFalse(ReferenceEquals(j1, j2));
        Assert.AreEqual(0, FullJidComparer.Shared.Compare(j1, j2));
    }
}
