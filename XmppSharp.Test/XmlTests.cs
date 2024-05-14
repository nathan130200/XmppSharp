using System.Diagnostics;
using XmppSharp.Dom;
using XmppSharp.Factory;
using XmppSharp.Protocol;
using XmppSharp.Protocol.Base;
using XmppSharp.Protocol.Sasl;

namespace XmppSharp.Test;

[TestClass]
public class XmlTests
{
	[TestMethod]
	public void EnumerateElementsFromRegistry()
	{
		EnumerateElementsFromRegistry<Iq>();
		EnumerateElementsFromRegistry<Message>();
		EnumerateElementsFromRegistry<Presence>();
		EnumerateElementsFromRegistry<StanzaError>();
		EnumerateElementsFromRegistry<Mechanism>();
		EnumerateElementsFromRegistry<Mechanisms>();
		EnumerateElementsFromRegistry<Auth>();
		EnumerateElementsFromRegistry<Success>();
		EnumerateElementsFromRegistry<Abort>();
		EnumerateElementsFromRegistry<StreamStream>();
		EnumerateElementsFromRegistry<StreamFeatures>();
		EnumerateElementsFromRegistry<StreamError>();
	}

	static void EnumerateElementsFromRegistry<T>() where T : Element
	{
		var values = ElementFactory.GetTags<T>();
		Debug.WriteLine("count: " + values.Count());

		foreach (var (tagName, uri) in values)
			Debug.WriteLine(" " + tagName + " (namespace: " + uri + ")");

		Debug.WriteLine("\n");
	}

	[TestMethod]
	[DataRow("stream:stream", Namespaces.Stream, typeof(StreamStream))]
	[DataRow("stream:features", Namespaces.Stream, typeof(StreamFeatures))]
	[DataRow("stream:error", Namespaces.Stream, typeof(StreamError))]

	[DataRow("iq", Namespaces.Client, typeof(Iq))]
	[DataRow("iq", Namespaces.Server, typeof(Iq))]
	[DataRow("iq", Namespaces.Accept, typeof(Iq))]
	[DataRow("iq", Namespaces.Connect, typeof(Iq))]

	[DataRow("message", Namespaces.Client, typeof(Message))]
	[DataRow("message", Namespaces.Server, typeof(Message))]
	[DataRow("message", Namespaces.Accept, typeof(Message))]
	[DataRow("message", Namespaces.Connect, typeof(Message))]

	[DataRow("presence", Namespaces.Client, typeof(Presence))]
	[DataRow("presence", Namespaces.Server, typeof(Presence))]
	[DataRow("presence", Namespaces.Accept, typeof(Presence))]
	[DataRow("presence", Namespaces.Connect, typeof(Presence))]

	[DataRow("iq", "foobar", typeof(Element))]
	[DataRow("message", "foobar", typeof(Element))]
	[DataRow("presence", "foobar", typeof(Element))]
	public void CreateElementFromFactory(string tagName, string uri, Type expectedType)
	{
		var elem = ElementFactory.Create(tagName, uri);
		Assert.AreEqual(elem.TagName, tagName);

		var ns = elem.Prefix == null
			? elem.GetNamespace()
			: elem.GetNamespace(elem.Prefix);

		Assert.AreEqual(ns, uri);
		Assert.IsInstanceOfType(elem, expectedType);
	}

	[TestMethod]
	[DataRow("<stream:stream xmlns='jabber:client' to='localhost' version='1.0' xmlns:stream='" + Namespaces.Stream + "' />", typeof(StreamStream))]
	[DataRow("<iq xmlns='jabber:client' />", typeof(Iq))]
	[DataRow("<message xmlns='jabber:component:accept' />", typeof(Message))]
	[DataRow("<presence xmlns='jabber:server' />", typeof(Presence))]
	[DataRow("<iq />", typeof(Iq))]
	[DataRow("<message />", typeof(Message))]
	[DataRow("<presence />", typeof(Presence))]
	[DataRow("<auth xmlns='" + Namespaces.Sasl + "' mechanism='PLAIN' />", typeof(Auth))]
	public async Task CreateElementFromParser(string xml, Type expectedType)
	{
		var elem = await ParserTests.ParseFromBuffer(xml);
		Assert.IsInstanceOfType(elem, expectedType);
		ParserTests.PrintResult(elem);
	}

	[TestMethod]
	public async Task CloneNodes()
	{
		var inXml = @"<mission name=""!!!Volcano_Nightmare"" time_of_day=""12"" game_mode=""pve"" game_mode_cfg=""pve_mode.cfg"" uid=""1bbe889f-cba6-41e2-9844-a2257d3ac25d"" release_mission=""0"" clan_war_mission=""0"" only_clan_war_mission=""0"" difficulty=""survival"" mission_type=""volcanosurvival"">
<Basemap name=""survival/africa_survival_base"" />
<UI>
<Description text=""@na_mission_volcano_desc_01"" icon=""mapImgNAvolcano_s"" />
<GameMode text=""@PvE_game_mode_desc"" />
</UI>
<Sublevels>
<Sublevel id=""0"" name=""survival/africa_survival"" mission_flow=""arena_flow"" score=""0"" difficulty=""survival"" difficulty_cfg=""volcano_survival.cfg"" win_pool=""150"" lose_pool=""50"" draw_pool=""55"" score_pool=""150"">
<RewardPools>
<Pool name=""0"" value=""0"" />
<Pool name=""1"" value=""80"" />
<Pool name=""2"" value=""100"" />
<Pool name=""3"" value=""110"" />
<Pool name=""4"" value=""130"" />
<Pool name=""5"" value=""140"" />
<Pool name=""6"" value=""160"" />
<Pool name=""7"" value=""170"" />
<Pool name=""8"" value=""180"" />
<Pool name=""9"" value=""190"" />
<Pool name=""10"" value=""200"" />
<Pool name=""11"" value=""210"" />
<Pool name=""12"" value=""230"" />
<Pool name=""13"" value=""0"" />
</RewardPools>
</Sublevel>
</Sublevels>
<Objectives>
<Objective type=""primary"" timelimit=""6000"" />
</Objectives>
<Teleports />
</mission>"
	.Replace("\r\n", "")
	.Replace("\n", "")
	.Replace("\r", "")
	.Replace("\t", "")
		;

		var elem = await ParserTests.ParseFromBuffer(inXml);

		var cloned = elem.Clone();
		Assert.AreNotSame(elem, cloned);

		var outXml = cloned.ToString(XmlFormatting.None);

		Assert.AreEqual(inXml, outXml);
	}

	[TestMethod]
	public void StartTagTest()
	{
		var el = new StreamStream();
		el.SetNamespace(Namespaces.Client);
		var xml = el.StartTag();
		Assert.AreEqual(xml, "<stream:stream xmlns=\"" + Namespaces.Client + "\" xmlns:stream=\"" + Namespaces.Stream + "\">");
		Console.WriteLine("START_TAG:\n" + xml);

		xml = el.EndTag();
		Assert.AreEqual(xml, "</stream:stream>");
		Console.WriteLine("END_TAG:\n" + xml);
	}
}