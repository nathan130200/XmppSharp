using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Xml;
using XmppSharp.Dom.Abstractions;
using XmppSharp.Serialization;

namespace XmppSharp.Dom;

public class Element : Container
{
	internal QName _elementName;
	internal readonly List<Attribute> _attributes = [];

	public Element(string name, string? ns, object? value = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		_elementName = new(XmlConvert.VerifyName(name));

		if (!_elementName.Prefix.IsEmpty)
			ArgumentException.ThrowIfNullOrWhiteSpace(ns);

		Namespace = ns;

		if (value != null)
			SetValue(value);
	}

	public string Name
	{
		get => _elementName.Name;
		internal set => _elementName = new(value);
	}

	public ReadOnlySpan<char> LocalName => _elementName.LocalName;

	public ReadOnlySpan<char> Prefix => _elementName.Prefix;

	public string? Namespace
	{
		get => GetNamespace(_elementName.Prefix);
		set => SetNamespace(_elementName.Prefix, value);
	}

	[NotNull]
	public string? InnerText
	{
		get => string.Concat(from n in Descendants().OfType<Text>()
							 select n.Value);
		set
		{
			RemoveNodes();

			if (value != null)
				AddChild(new Text(value));
		}
	}

	public void RemoveAttributes()
	{
		lock (_attributes)
		{
			foreach (var attr in _attributes)
				attr._parent = null;

			_attributes.Clear();
		}
	}

	public void RemoveAll()
	{
		RemoveAttributes();
		RemoveNodes();
	}

	public void SetValue(object? value, IFormatProvider? fmt = default)
	{
		RemoveNodes();

		if (value != null)
			AddChild(new Text(value, fmt));
	}

	public void AddChild(Node? node)
	{
		if (node is null) return;

		if (node._parent != null)
			node = node.Clone();

		lock (_children)
		{
			_children.Add(node);

			node._parent = this;
		}
	}

	public void RemoveChild(Node? node)
	{
		if (node?._parent != this) return;

		lock (_children)
		{
			_children.Remove(node);

			node._parent = null;
		}
	}

	public override Node Clone()
	{
		var result = ElementFactory.CreateElement(Name, Namespace);

		lock (_attributes)
		{
			foreach (var attr in _attributes)
			{
				var temp = attr.Clone();
				temp._parent = result;
				result._attributes.Add(temp);
			}
		}

		lock (_children)
		{
			foreach (var node in _children)
			{
				var temp = node.Clone();
				temp._parent = result;
				result._children.Add(temp);
			}
		}

		return result;
	}

	public bool HasAttribute(string name)
	{
		lock (_attributes)
			return _attributes.FindIndex(x => x.Name == name) != -1;
	}

	public string? GetAttribute(string name)
	{
		lock (_attributes)
			return _attributes.Find(x => x.Name == name)?.Value;
	}

	internal void RemoveAttributeInternal(Attribute? attr)
	{
		lock (_attributes)
		{
			if (attr?._parent != this)
				return;

			_attributes.Remove(attr);

			attr._parent = null;
		}
	}

	public bool RemoveAttribute(string name)
	{
		lock (_attributes)
		{
			var entry = _attributes.Find(x => x.Name == name);

			if (entry != null)
			{
				entry._parent = null;

				_attributes.Remove(entry);

				return true;
			}
		}

		return false;
	}

	public void SetAttribute(string name, object? value, IFormatProvider? fmt = default)
	{
		if (value is null)
		{
			RemoveAttribute(name);
			return;
		}

		lock (_attributes)
		{
			var entry = _attributes.Find(x => x.Name == name);

			var newValue = Convert.ToString(value, fmt ?? CultureInfo.InvariantCulture);

			if (entry != null)
				entry.Value = newValue;
			else
			{
				entry = new Attribute(name, newValue)
				{
					_parent = this
				};

				_attributes.Add(entry);
			}
		}
	}

	public string? GetNamespace(ReadOnlySpan<char> prefix)
	{
		var name = prefix.IsEmpty ? "xmlns" : $"xmlns:{prefix}";

		if (GetAttribute(name) is string inScope)
			return inScope;

		if (_parent?.GetNamespace(prefix) is string inParent)
			return inParent;

		return null;
	}

	public void SetNamespace(ReadOnlySpan<char> prefix, string? ns)
	{
		var name = prefix.IsEmpty ? "xmlns" : $"xmlns:{prefix}";

		if (ns is null)
			RemoveAttribute(name);
		else
			SetAttribute(name, ns);
	}

	protected void WriteStartElement(XmlWriter writer)
	{
		var (prefix, localName) = _elementName;

		writer.WriteStartElement(prefix, localName, Namespace);

		lock (_attributes)
		{
			foreach (var attr in _attributes)
				attr.WriteTo(writer);
		}
	}

	protected void WriteContent(XmlWriter writer)
	{
		lock (_children)
		{
			foreach (var node in _children)
				node.WriteTo(writer);
		}
	}

	public override void WriteTo(XmlWriter writer)
	{
		WriteStartElement(writer);
		WriteContent(writer);
		writer.WriteEndElement();
	}

	public void Save(Stream stream, XmlWriterSettings? settings = default)
		=> WriteTo(stream, settings);

	public void Save(TextWriter textWriter, XmlWriterSettings? settings = default)
		=> WriteTo(textWriter, settings);

	public void Save(string fileName, XmlWriterSettings? settings = default)
	{
		using var stream = File.Open(fileName, FileMode.OpenOrCreate);

		stream.SetLength(0);

		WriteTo(stream, settings);
	}

	void WriteTo(dynamic output, XmlWriterSettings? settings)
	{
		settings ??= new XmlWriterSettings()
		{
			ConformanceLevel = ConformanceLevel.Document,
			Indent = false,
			CloseOutput = true,
			Encoding = Encoding.UTF8,
			NamespaceHandling = NamespaceHandling.OmitDuplicates,
			WriteEndDocumentOnClose = false
		};

		using var writer = XmlWriter.Create(output, settings);

		WriteTo(writer);
	}

	public bool HasTag(string name, string? ns = null)
		=> Element(name, ns) != null;

	public bool RemoveTag(string name, string? ns = null)
	{
		bool changed = false;

		do
		{


			if (Element(name, ns) is not Element e)
				break;

			e.Remove();

			changed = true;
		} while (true);

		return changed;
	}

	public string? GetTag(string name, string? ns = null)
		=> Element(name, ns)?.InnerText;

	public Element SetTag(string name, string? ns = null, object? value = default)
	{
		var el = Element(name, ns);

		if (el == null)
		{
			el = ElementFactory.CreateElement(name, ns);
			AddChild(el);
		}

		el.SetValue(value);

		return el;
	}

	public T GetAttribute<T>(string name, T defaultValue, IFormatProvider? fmt = default) where T : IParsable<T>
	{
		var rawValue = GetAttribute(name);

		if (rawValue != null)
		{
			if (T.TryParse(rawValue, fmt ?? CultureInfo.InvariantCulture, out var result))
				return result;
		}

		return defaultValue;
	}

	public T? GetAttribute<T>(string name, IFormatProvider? fmt = default) where T : struct, IParsable<T>
	{
		var rawValue = GetAttribute(name);

		if (rawValue != null)
		{
			if (T.TryParse(rawValue, fmt ?? CultureInfo.InvariantCulture, out var result))
				return result;
		}

		return default;
	}

	public sealed override string ToString() => ToString(false);

	public virtual string ToString(bool indented)
	{
		using var sb = new StringWriterWithEncoding();

		var settings = new XmlWriterSettings
		{
			ConformanceLevel = ConformanceLevel.Fragment,
			Indent = indented,
			CloseOutput = true,
			Encoding = sb.Encoding,
			NamespaceHandling = NamespaceHandling.OmitDuplicates,
			WriteEndDocumentOnClose = false
		};

		using (var writer = XmlWriter.Create(sb, settings))
			WriteTo(writer);

		return sb.ToString();
	}
}