using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Extensions.XEP0030;

/// <summary>
/// This is a implementation of disco items extension.
/// </summary>
[StanzaExtension("query", Namespaces.DiscoInfo)]
public class DiscoInfo : BaseStanzaExtension
{
	IEnumerable<Identity>? _identities;
	IEnumerable<string>? _features;

	/// <inheritdoc/>
	public DiscoInfo()
	{

	}

	internal DiscoInfo(Element element) : base(element)
	{
		Node = element.Attribute("node");

		{
			List<string>? features = null;

			foreach (var child in element.Elements("feature"))
			{
				string? feature = child.Attribute("var");

				if (string.IsNullOrWhiteSpace(feature))
					continue;

				features ??= [];

				features.Add(feature);
			}

			_features = features;
		}

		{
			List<Identity>? temp = null;

			foreach (var child in element.Elements("identity"))
			{
				string? category = child.Attribute("category");
				string? type = child.Attribute("type");

				if (string.IsNullOrWhiteSpace(category) || string.IsNullOrWhiteSpace(type))
					continue;

				string? name = child.Attribute("name");

				temp ??= [];

				temp.Add(new(category, type, name));
			}

			_identities = temp;
		}
	}

	/// <summary>
	/// Gets or sets the node identifier.
	/// </summary>
	public string? Node { get; set; }

	/// <summary>
	/// Gets or sets supported features by the entity.
	/// </summary>
	public IEnumerable<string> Features
	{
		get
		{
			lock (this)
			{
				if (_features is null) return [];

				return _features;
			}
		}
		set
		{
			lock (this)
			{
				if (value is null)
					_features = null;
				else
					_features = [.. value];
			}
		}
	}

	/// <summary>
	/// Gets or sets identities of the entity.
	/// </summary>
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
		set
		{
			lock (this)
				_identities = value;
		}
	}

	/// <inheritdoc/>
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