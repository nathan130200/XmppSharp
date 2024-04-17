﻿using XmppSharp.Attributes;

namespace XmppSharp.Protocol.ServiceDiscovery;

[XmppTag("feature", Namespace.DiscoInfo)]
public class Feature : Element
{
	public Feature() : base("feature", Namespace.DiscoInfo)
	{

	}

	public Feature(string featureName) : this()
	{
		this.Name = featureName;
	}

	public string Name
	{
		get => this.GetAttribute("var");
		set => this.SetAttribute("var", value);
	}
}