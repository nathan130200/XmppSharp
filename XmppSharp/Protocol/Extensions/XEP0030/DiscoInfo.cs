using XmppSharp.Abstractions;
using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

[StanzaExtension("query", Namespaces.DiscoInfo)]
public class DiscoInfo : StanzaExtension
{
	List<Identity>? _identities;
	HashSet<string>? _features;

	public DiscoInfo()
	{

	}

	protected DiscoInfo(Element element) : base(element)
	{
		foreach (var tag in element.Elements("feature"))
		{
			string? feature = tag.Attribute("var");

			if (string.IsNullOrWhiteSpace(feature))
				continue;

			_features ??= [];

			_features.Add(feature);
		}

		foreach (var tag in element.Elements("identity"))
		{
			string? category = tag.Attribute("category");
			string? type = tag.Attribute("type");
			string? name = tag.Attribute("name");

			if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(type))
				continue;

			_identities ??= [];

			_identities.Add(new(category, type, name));
		}
	}

	public void AddFeature(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		lock (this)
		{
			_features ??= [];
			_features.Add(name);
		}
	}

	public void RemoveAllFeatures()
	{
		lock (this)
			_features?.Clear();
	}

	public bool HasFeature(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		lock (this)
		{
			if (_features == null) return false;

			return _features.Contains(name);
		}
	}

	public void AddIdentity(Identity identity)
	{
		ArgumentNullException.ThrowIfNull(identity);

		lock (this)
		{
			_identities ??= [];

			_identities.Add(identity);
		}
	}

	public void AddIdentity(string category, string type, string? name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(category);

		ArgumentException.ThrowIfNullOrWhiteSpace(type);

		lock (this)
		{
			_identities ??= [];

			_identities.Add(new(category, type, name));
		}
	}

	public IEnumerable<string> Features
	{
		get
		{
			lock (this)
			{
				if (_features is null)
					return [];

				return [.. _features];
			}
		}
	}

	public IEnumerable<Identity> Identities
	{
		get
		{
			lock (this)
			{
				if (_identities is null)
					return [];

				return [.. _identities];
			}
		}
	}

	public override Element Serialize()
	{
		var result = new Element("query", Namespaces.DiscoInfo);

		lock (this)
		{
			if (_identities != null)
			{
				foreach (var identity in _identities)
				{
					result.C("identity", x =>
					{
						x.SetAttribute("category", identity.Category);
						x.SetAttribute("type", identity.Type);
						x.SetAttribute("name", identity.Name);
					});
				}
			}

			if (_features != null)
			{
				foreach (var featureName in _features)
					result.C("feature", x => x.SetAttribute("var", featureName));
			}
		}

		return result;
	}
}