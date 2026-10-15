namespace XmppSharp.Protocol.Extensions.XEP0030;

/// <summary>
/// Represents a item entry in disco items query.
/// </summary>
public class Item : IEquatable<Item>
{
	/// <summary>
	/// Gets the item's jid.
	/// </summary>
	public Jid Jid { get; }

	/// <summary>
	/// Gets the item's name.
	/// </summary>
	public string? Name { get; }

	/// <summary>
	/// Gets the item's node.
	/// </summary>
	public string? Node { get; }

	readonly int _hashCode;

	/// <summary>
	/// Initialize a new instance of <see cref="Item"/> class.
	/// </summary>
	/// <param name="jid">The item jid.</param>
	/// <param name="name">The optional item name.</param>
	/// <param name="node">The optional item node.</param>
	public Item(Jid jid, string? name = default, string? node = default)
	{
		ArgumentNullException.ThrowIfNull(jid);

		Jid = jid;

		Name = name;

		Node = node;

		_hashCode = HashCode.Combine(jid, name, node);
	}

	/// <inheritdoc/>
	public override int GetHashCode() => _hashCode;

	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is Item other && Equals(other);

	/// <inheritdoc/>
	public bool Equals(Item? other)
	{
		if (other is null) return false;

		return Jid.Equals(other.Jid)
			&& string.Equals(Name, other.Name)
			&& string.Equals(Node, other.Node);
	}

	/// <inheritdoc />
	public override string ToString() => $"Item(Jid='{Jid}'; Name='{Name}'; Node='{Node}')";

	/// <inheritdoc />
	public static bool operator !=(Item? x, Item? y) => !(x == y);

	/// <inheritdoc />
	public static bool operator ==(Item? x, Item? y)
	{
		if (x is null) return y is null;

		if (ReferenceEquals(x, y)) return true;

		return x.Equals(y);
	}
}