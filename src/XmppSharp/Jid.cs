using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace XmppSharp;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct Jid
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly string? _user, _resource;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly string _server = default!;

	public Jid()
	{
		_user = _server = _resource = null!;
	}

	public Jid(string? user, string server, string? resource)
	{
		User = user;
		Server = server;
		Resource = resource;
	}

	public string? User
	{
		get => _user;
		init => _user = Validate(EscapeNode(value));
	}

	public string Server
	{
		get => _server;
		init
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(Server));
			_server = Validate(value.ToLowerInvariant())!;
		}
	}

	public string? Resource
	{
		get => _resource;
		init => Validate(_resource = HttpUtility.UrlEncode(value));
	}

	static string? Validate(string? value, [CallerMemberName] string memberName = default!)
	{
		foreach (char c in value.AsSpan())
		{
			if (!char.IsAscii(c))
				throw new ArgumentException($"Jid '{memberName}' contains non-ASCII chars.");
		}

		return value;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public bool IsEmpty
		=> string.IsNullOrEmpty(_user)
		&& string.IsNullOrEmpty(_server)
		&& string.IsNullOrEmpty(_resource);

	public override int GetHashCode()
	{
		int hash = _server?.GetHashCode() ?? 0;

		if (!string.IsNullOrEmpty(_user))
			hash = HashCode.Combine(hash, _user);

		if (!string.IsNullOrEmpty(_resource))
			hash = HashCode.Combine(hash, _resource);

		return hash;
	}

	static string? EscapeNode(string? node)
	{
		if (string.IsNullOrEmpty(node))
			return node;

		var sb = new StringBuilder();

		for (int i = 0; i < node.Length; i++)
		{
			char c = node[i];

			switch (c)
			{
				case ' ': sb.Append(@"\20"); break;
				case '"': sb.Append(@"\22"); break;
				case '&': sb.Append(@"\26"); break;
				case '\'': sb.Append(@"\27"); break;
				case '/': sb.Append(@"\2f"); break;
				case ':': sb.Append(@"\3a"); break;
				case '<': sb.Append(@"\3c"); break;
				case '>': sb.Append(@"\3e"); break;
				case '@': sb.Append(@"\40"); break;
				case '\\': sb.Append(@"\5c"); break;
				default: sb.Append(c); break;
			}
		}

		return sb.ToString();
	}

	[StackTraceHidden]
	static void CheckByteSize(string? value, string memberName)
	{
		if (string.IsNullOrWhiteSpace(value))
			return;

		var numBytes = Encoding.UTF8.GetByteCount(value);

		if (numBytes > 1023)
			throw new ArgumentException($"Jid '{memberName}' cannot be 1023 bytes long.");
	}

	public override string ToString()
	{
		CheckByteSize(_user, nameof(User));
		CheckByteSize(_server, nameof(Server));
		CheckByteSize(_resource, nameof(Resource));

		var sb = new StringBuilder();

		if (!string.IsNullOrEmpty(_user))
			sb.Append(_user).Append('@');

		if (!string.IsNullOrEmpty(_server))
			sb.Append(_server);

		if (!string.IsNullOrEmpty(_resource))
			sb.Append('/').Append(_resource);

		return sb.ToString();
	}

	public override bool Equals(object? obj)
	{
		if (obj is not Jid other)
			return false;

		if (IsEmpty || other.IsEmpty)
			return false;

		return string.Equals(_user, other._user)
			&& string.Equals(_server, other._server)
			&& string.Equals(_resource, other._resource);
	}

	public bool IsBare
		=> string.IsNullOrEmpty(_resource);

	public Jid Bare
		=> this with { Resource = null };

	public static Jid Parse(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
			return default;

		var ofs = text.IndexOf('@');

		string? user = null,
			resource = null;

		if (ofs != -1)
		{
			user = text[0..ofs];
			text = text[(ofs + 1)..];
		}

		ofs = text.IndexOf('/');

		string server;

		if (ofs == -1)
			server = text;
		else
		{
			server = text[0..ofs];
			resource = text[(ofs + 1)..];
		}

		ArgumentException.ThrowIfNullOrWhiteSpace(server);

		return new Jid { User = user, Server = server, Resource = resource };
	}

	public static implicit operator string(Jid j) => j.ToString();
	public static bool operator ==(Jid left, Jid right) => left.Equals(right);
	public static bool operator !=(Jid left, Jid right) => !(left == right);
}
