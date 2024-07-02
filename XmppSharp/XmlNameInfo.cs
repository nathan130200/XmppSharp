using System.Diagnostics;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public readonly record struct XmlNameInfo
{
    public bool HasPrefix 
        => !string.IsNullOrWhiteSpace(this.Prefix);

    public required string LocalName
	{
		get;
		init;
	}

    public string? Prefix
	{
		get;
		init;
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
