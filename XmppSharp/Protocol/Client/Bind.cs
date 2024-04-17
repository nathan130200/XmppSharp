using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Client;

[XmppTag("bind", Namespace.Bind)]
public class Bind : Element
{
	public Bind() : base("bind", Namespace.Bind)
	{

	}

	public Bind(string resource) : this()
		=> this.Resource = resource;

	public Bind(Jid jid) : this()
		=> this.Jid = jid;

	public string Resource
	{
		get => this.GetTag("resource");
		set
		{
			if (value == null)
				this.RemoveTag("resource");
			else
				this.SetTag("resource", value);
		}
	}

	public Jid Jid
	{
		get
		{
			var jid = this.GetTag("jid");

			if (Jid.TryParse(jid, out var result))
				return result;

			return null;
		}
		set
		{
			if (value == null)
				this.RemoveTag("jid");
			else
				this.SetTag("jid", value.ToString());
		}
	}
}
