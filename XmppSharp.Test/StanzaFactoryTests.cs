using XmppSharp.Dom;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Client;
using XmppSharp.Protocol.Extensions.XEP0030;
using XmppSharp.Serialization;
using Xunit.Abstractions;

namespace XmppSharp.Test;

public class StanzaFactoryTests(ITestOutputHelper Output)
{
	[Theory]
	[InlineData("bind", Namespaces.Bind, typeof(Bind))]
	[InlineData("session", Namespaces.Session, typeof(Session))]
	[InlineData("query", Namespaces.DiscoInfo, typeof(DiscoInfo))]
	public void TryCreateStanzaExtension(string localName, string uri, Type expectedType)
	{
		var ext = StanzaExtensionFactory.Create(new Element(localName, uri));

		Assert.NotNull(ext);

		Assert.IsType(expectedType, ext);

		Output.WriteLine("XML: " + ext.Serialize());
	}

	[Theory]
	[InlineData(ErrorType.Modify, ErrorCondition.BadRequest, Namespaces.Client)]
	[InlineData(ErrorType.Wait, ErrorCondition.ServiceUnavailable, Namespaces.Server)]
	[InlineData(ErrorType.Continue, ErrorCondition.RecipientUnavailable, Namespaces.Component)]
	public void TryCreateStanzaErrorExtension(ErrorType type, ErrorCondition cond, string uri)
	{
		var el = new Element("error", uri);
		el.SetAttribute("type", XmppEnumMapping<ErrorType>.Shared.GetName(type));
		el.SetTag(XmppEnumMapping<ErrorCondition>.Shared.GetName(cond), Namespaces.Stanzas);

		var ext = StanzaExtensionFactory.Create(el) as Error;

		Assert.NotNull(ext);

		Assert.Equal(type, ext.Type);

		Assert.Equal(cond, ext.Condition);

		Output.WriteLine("XML: " + ext.Serialize());
	}

	[Fact]
	public void TryDeserializeDiscoInfo()
	{
		var feature = "http://jabber.org/protocol/muc";

		DiscoInfo ext;

		{
			var element = new Element("query", Namespaces.DiscoInfo);

			element.C("identity", x =>
			{
				x.SetAttribute("category", "conference");
				x.SetAttribute("type", "text");
			});

			element.C("feature", x =>
			{
				x.SetAttribute("var", feature);
			});

			ext = StanzaExtensionFactory.Create<DiscoInfo>(element);
		}

		Assert.True(ext.HasFeature(feature));

		var identity = ext.Identities.FirstOrDefault();

		Assert.NotNull(identity);

		Assert.Equal("conference", identity.Category);

		Assert.Equal("text", identity.Type);

		Output.WriteLine("XML: " + ext.Serialize());
	}

	[Theory]
	[InlineData(ErrorType.Modify, ErrorCondition.BadRequest)]
	[InlineData(ErrorType.Wait, ErrorCondition.ServiceUnavailable)]
	[InlineData(ErrorType.Continue, ErrorCondition.RecipientUnavailable)]
	public void TrySerializeStanzaErrorExtension(ErrorType type, ErrorCondition cond)
	{
		var ext = new Error();

		ext.Type = type;

		ext.Condition = cond;

		var tag = ext.Serialize()!;

		Assert.NotNull(tag);

		var errorTypeName = XmppEnumMapping<ErrorType>.Shared.GetName(type);

		var errorConditionName = XmppEnumMapping<ErrorCondition>.Shared.GetName(cond);

		Assert.Equal(errorTypeName, tag.Attribute("type"));

		Assert.NotNull(tag.Element(errorConditionName, Namespaces.Stanzas));

		Output.WriteLine("XML: " + tag.ToString());
	}

	[Fact]
	public void TestAttributeParsing_UInt32()
	{
		var attr = new Dom.Attribute("theory", 1234U);

		var parsed = (uint?)attr;

		Assert.True(parsed.HasValue);

		Assert.Equal(1234U, parsed.Value);
	}

	[Fact]
	public void TestAttributeParsing_Float()
	{
		var attr = new Dom.Attribute("theory", 35.35f);

		var parsed = (float?)attr;

		Assert.True(parsed.HasValue);

		Assert.Equal(35.35f, parsed.Value);
	}

	[Fact]
	public void TestAttributeParsing_Bool()
	{
		var attr = new Dom.Attribute("theory", 1);

		var parsed = (bool?)attr;

		Assert.True(parsed.HasValue);

		Assert.True(parsed.Value);
	}
}
