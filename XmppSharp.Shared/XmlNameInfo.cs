using System.Diagnostics;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public readonly record struct XmlNameInfo
{
	public bool HasPrefix
		=> !string.IsNullOrWhiteSpace(this.Prefix);

	public

#if NET7_0_OR_GREATER
		required
#endif

		string LocalName
	{
		get;
		init;
	}

	public string? Prefix
	{
		get;
		init;
	}

	public void Deconstruct(out string localName, out string prefix)
	{
		localName = LocalName;
		prefix = Prefix;
	}

	public void Deconstruct(out string localName, out string prefix, out bool hasPrefix)
	{
		localName = LocalName;
		prefix = Prefix;
		hasPrefix = HasPrefix;
	}

	public override string ToString()
	{
		if (string.IsNullOrWhiteSpace(Prefix))
			return LocalName;

		return string.Concat(Prefix, ':', LocalName);
	}

	public static implicit operator string(XmlNameInfo qname)
		=> qname.ToString();

	public static implicit operator XmlNameInfo(string str)
	{
		var ofs = str.IndexOf(':');

		string localName, prefix = default;

		if (ofs > 0)
			prefix = str[0..ofs];

		localName = str[(ofs + 1)..];

		return new XmlNameInfo
		{
			LocalName = localName,
			Prefix = prefix
		};
	}
}
