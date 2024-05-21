using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using XmppSharp.Dom;
using XmppSharp.Protocol.Sasl;
using XmppSharp.Protocol.ServiceDiscovery.IdentityValues;
using XmppSharp.Protocol.Tls;

namespace XmppSharp.Test;

[TestClass]
public class AttributeHelperTests
{
	Element element = default!;

	[TestInitialize]
	public void Initialize()
		=> element = new("root");

	[TestCleanup]
	public void Cleanup()
		=> element = null!;

	[TestMethod]
	public void WithXmppEnum()
	{
		WithXmppEnum(TlsPolicy.Required, "required");
		WithXmppEnum(TlsPolicy.Optional, "optional");
		WithXmppEnum(MechanismType.Plain, "PLAIN");
		WithXmppEnum(MechanismType.DigestMD5, "DIGEST-MD5");
		WithXmppEnum(ConferenceValues.Text, "text");
		WithXmppEnum(PubSubValues.Owned, "pep");
		WithXmppEnum(PubSubValues.Service, "service");
		WithXmppEnum(GatewayValues.Discord, "discord");
		WithXmppEnum(GatewayValues.Facebook, "facebook");
		WithXmppEnum(GatewayValues.AIM, "aim");
	}

	void WithXmppEnum<T>(T value, string rawValue) where T : struct, Enum
	{
		Debug.WriteLine($"WithXmppEnum<{typeof(T).Name}>: raw value={rawValue}");

		element.SetAttribute("data", value.ToXmppName());
		Assert.AreEqual(rawValue, element.GetAttribute("data"));

		var actual = XmppEnum.ParseOrThrow<T>(element.GetAttribute("data")!);
		Assert.AreEqual(value, actual);

		Debug.WriteLine($"  {typeof(T).Name}.{actual}: parsed? {value.Equals(actual)}");
		XmlReaderParserTests.PrintResult(element);
		Debug.WriteLine(" ---------------------------------\n");
	}

	enum EnumTestType
	{
		CSharp,
		VisualBasic
	}

	[TestMethod]
	public void WithBitwise()
	{
		WithBitwise(FileAccess.ReadWrite, true);
		WithBitwise(SslProtocols.Tls12 | SslProtocols.Tls13, true);
	}

	[TestMethod]
	public void WithBitwiseAsString()
	{
		WithBitwise(FileAccess.ReadWrite, false);
		WithBitwise(SslProtocols.Tls12 | SslProtocols.Tls13, false);
	}

	[TestMethod]
	public void WithString()
	{
		WithBitwise(EnumTestType.CSharp, false);
		WithBitwise(EnumTestType.VisualBasic, false);
		WithBitwise(ConferenceValues.Text, false);
		WithBitwise(TestTimeout.Infinite, false);
		WithBitwise(TestIdGenerationStrategy.FullyQualified, false);
		WithBitwise(ThreadWaitReason.ExecutionDelay, false);
		WithBitwise(ThreadWaitReason.LpcReply, false);
		WithBitwise(ThreadPriority.AboveNormal, false);
		WithBitwise(ThreadPriority.Highest, false);
	}

	void WithBitwise<T>(T value, bool isNumber, [CallerMemberName] string func = default!) where T : struct, Enum
	{
		var typeName = typeof(T).Name;
		object rawValue = Convert.ChangeType(value, Enum.GetUnderlyingType(typeof(T)));

		element.SetAttributeEnum("data", (T?)value, isNumber);
		Assert.AreEqual(isNumber ? rawValue.ToString() : value.ToString(), element.GetAttribute("data"));

		var actual = element.GetAttributeEnum<T>("data", isNumber: isNumber);
		Debug.WriteLine($"{func}<{typeName}>: {value} ({rawValue} @ {rawValue.GetType().Name})");

		var isParsed = value.Equals(actual);

		Debug.WriteLine($"  {typeName}.{actual}: parsed? {isParsed}\n");

		XmlReaderParserTests.PrintResult(element);

		Debug.WriteLine("\n---------------------------------\n");

		if (!isParsed)
			Assert.Fail();
	}
}
