namespace XmppSharp.Protocol.Extensions.XEP0030;

/// <summary>
/// Represents a identity item in disco items query.
/// </summary>
public sealed class Identity : IEquatable<Identity>
{
	/// <summary>
	/// The identity's category.
	/// </summary>
	public string Category { get; }

	/// <summary>
	/// The identity's type.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// The identity's name.
	/// </summary>
	public string? Name { get; }

	readonly int _hashCode;

	/// <summary>
	/// Initialize a new instance of <see cref="Identity"/> class.
	/// </summary>
	/// <param name="category">The identity's category.</param>
	/// <param name="type">The identity's type.</param>
	/// <param name="name">The optional identity's name.</param>
	public Identity(string category, string type, string? name = default)
	{
		Category = category;

		Type = type;

		Name = name;

		_hashCode = HashCode.Combine(category, type, name);
	}

	/// <inheritdoc/>
	public override int GetHashCode() => _hashCode;

	/// <inheritdoc/>
	public override bool Equals(object? obj) => obj is Identity other && Equals(other);

	/// <inheritdoc/>
	public bool Equals(Identity? other)
	{
		if (other is null) return false;

		return string.Equals(Category, other.Category)
			&& string.Equals(Type, other.Type)
			&& string.Equals(Name, other.Name);
	}

	/// <inheritdoc />
	public static bool operator !=(Identity? x, Identity? y) => !(x == y);

	/// <inheritdoc />
	public static bool operator ==(Identity? x, Identity? y)
	{
		if (x is null) return y is null;

		if (ReferenceEquals(x, y)) return true;

		return x.Equals(y);
	}
}
