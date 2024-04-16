using System.Diagnostics;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.ServiceDiscovery.IdentityValues;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Test;

[TestClass]
public class XmppEnumTests
{
    [TestMethod]
    public void ParseOrThrow()
    {
        ParseFromString(IqType.Get, "get");
        ParseFromString(IqType.Set, "set");
        ParseFromString(IqType.Result, "result");
        ParseFromString(IqType.Error, "error");
        ParseFromString(MechanismType.Plain, "PLAIN");
        ParseFromString(MechanismType.DigestMD5, "DIGEST-MD5");
        ParseFromString(MechanismType.ScramSha1, "SCRAM-SHA-1");
        ParseFromString(MechanismType.ScramSha1Plus, "SCRAM-SHA-1-PLUS");
        ParseFromString(MechanismType.External, "EXTERNAL");
        ParseFromString(TlsPolicy.Required, "required");
        ParseFromString(TlsPolicy.Optional, "optional");
        ParseFromString(ComponentValues.C2S, "c2s");
        ParseFromString(ComponentValues.Router, "router");
        ParseFromString(ComponentValues.S2S, "s2s");
        Assert.ThrowsException<XmppEnumException>(() => XmppEnum.ParseOrThrow<MechanismType>("UNSPECIFIED"));
        Assert.IsTrue(XmppEnum.Parse<MechanismType>("UNSPECIFIED") == null);
    }

    static void ParseFromString<T>(T expected, string source) where T : struct, Enum
    {
        var actual = XmppEnum.ParseOrThrow<T>(source);
        Assert.AreEqual(actual, expected);
        Debug.WriteLine("ParseFromString(" + typeof(T).Name + "): " + source + " = " + expected);
    }

    [TestMethod]
    public void ToXmppName()
    {
        ToXmppName(IqType.Get, "get");
        ToXmppName(IqType.Set, "set");
        ToXmppName(IqType.Result, "result");
        ToXmppName(IqType.Error, "error");
        ToXmppName(MechanismType.Plain, "PLAIN");
        ToXmppName(MechanismType.DigestMD5, "DIGEST-MD5");
        ToXmppName(MechanismType.ScramSha1, "SCRAM-SHA-1");
        ToXmppName(MechanismType.ScramSha1Plus, "SCRAM-SHA-1-PLUS");
        ToXmppName(MechanismType.External, "EXTERNAL");
        ToXmppName(MechanismType.Unspecified, null!);
        ToXmppName(TlsPolicy.Required, "required");
        ToXmppName(TlsPolicy.Optional, "optional");
        ToXmppName(ComponentValues.C2S, "c2s");
        ToXmppName(ComponentValues.Router, "router");
        ToXmppName(ComponentValues.S2S, "s2s");
    }

    static void ToXmppName<T>(T value, string expected) where T : struct, Enum
    {
        var actual = XmppEnum.ToXmppName(value);
        Assert.AreEqual(expected, actual, true);
        Debug.WriteLine("ToXmppName(" + typeof(T).Name + "." + value + "): " + (expected ?? "<null>"));
    }
}