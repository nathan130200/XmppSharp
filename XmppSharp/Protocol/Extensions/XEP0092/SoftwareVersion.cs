using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0092;

public class SoftwareVersion : XmppElement
{
    public SoftwareVersion() : base("query", Namespaces.IqVersion)
    {

    }

    /// <summary>
    /// The natural-language name of the software.
    /// </summary>
    public string? Name
    {
        get => GetTag("name");
        set
        {
            if (value == null)
                RemoveTag("name");
            else
                SetTag("name", value: value);
        }
    }

    /// <summary>
    /// The specific version of the software.
    /// </summary>
    public string? Version
    {
        get => GetTag("version");
        set
        {
            if (value == null)
                RemoveTag("version");
            else
                SetTag("version", value: value);
        }
    }

    /// <summary>
    /// The operating system of the queried entity.
    /// </summary>
    public string? OperatingSystem
    {
        get => GetTag("os");
        set
        {
            if (value == null)
                RemoveTag("os");
            else
                SetTag("os", value: value);
        }
    }
}
