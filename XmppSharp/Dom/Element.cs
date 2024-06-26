﻿using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using XmppSharp.Factory;

namespace XmppSharp.Dom;

[DebuggerDisplay("{" + nameof(StartTag) + "(),nq}")]
public partial class Element : Node
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private string _localName, _prefix;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Dictionary<string, string> _attributes = new();

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly List<Node> _childNodes = new();

	Element()
	{

	}

	public Element(string qualifiedName)
	{
		var info = Xml.ExtractQualifiedName(qualifiedName);
		this._localName = info.LocalName;
		this._prefix = info.HasPrefix ? info.Prefix : null;
	}

	public Element(string qualifiedName, string namespaceURI) : this(qualifiedName)
	{
		if (namespaceURI != null)
		{
			if (this._prefix != null)
				this.SetNamespace(this._prefix, namespaceURI);
			else
				this.SetNamespace(namespaceURI);
		}
	}

	public bool IsRootElement
		=> Parent is null;

	public IEnumerable<Node> Nodes()
	{
		lock (this._childNodes)
			return this._childNodes.ToArray();
	}

	static void GetDescendantNodes(Element parent, List<Node> result)
	{
		foreach (var node in parent.Nodes())
		{
			result.Add(node);

			if (node is Element e)
				GetDescendantNodes(e, result);
		}
	}

	static void GetDescendantElements(Element parent, List<Element> result)
	{
		foreach (var element in parent.Children())
		{
			result.Add(element);

			GetDescendantElements(element, result);
		}
	}

	public IEnumerable<Node> DescendantNodes()
	{
		var result = new List<Node>();
		GetDescendantNodes(this, result);
		return result;
	}

	public IEnumerable<Node> DescendantNodesAndSelf()
	{
		var result = new List<Node> { this };
		GetDescendantNodes(this, result);
		return result;
	}

	public IEnumerable<Element> Descendants()
	{
		var result = new List<Element>();
		GetDescendantElements(this, result);
		return result;
	}

	public IEnumerable<Element> DescendantsAndSelf()
	{
		var result = new List<Element> { this };
		GetDescendantElements(this, result);
		return result;
	}

	public override string Value
	{
		get => string.Concat(this.Nodes()
			.OfType<Text>()
			.Select(x => x.Value));
		set
		{
			this.Nodes().Remove();

			if (value != null)
				this.AddChild(new Text(value));
		}
	}

	/// <summary>
	/// Gets the XML string representation of the current element and its child nodes.
	/// </summary>
	/// <returns>Well-formed XML serialized with the entire XML tree.</returns>
	public override string ToString()
		=> this.ToString(XmlFormatting.None);

	/// <summary>
	/// Gets the XML string representation of the current element and its child nodes.
	/// </summary>
	/// <param name="indented">Determines if result xml will be indented.</param>
	/// <returns>Well-formed XML serialized with the entire XML tree.</returns>
	public string ToString(bool indented)
		=> this.ToString(indented ? XmlFormatting.Indented : XmlFormatting.None);

	/// <summary>
	/// Gets the XML string representation of the current element and its child nodes.
	/// </summary>
	/// <param name="formatting">Determines which formatting will be used.</param>
	/// <returns>Well-formed XML serialized with the entire XML tree.</returns>
	public string ToString(XmlFormatting formatting)
	{
		var sb = new StringBuilder();

		using (var writer = Xml.CreateWriter(sb, formatting))
			this.WriteTo(writer, formatting);

		return sb.ToString();
	}

	public string? Namespace
	{
		get => this.GetNamespace();
		set => this.SetNamespace(value);
	}

	public Element Clone(bool deep)
	{
		var elem = ElementFactory.Create(this.TagName, this.GetNamespace(this.Prefix));
		elem._localName = this._localName;
		elem._prefix = this._prefix;

		lock (this._attributes)
		{
			foreach (var (key, value) in this._attributes)
				elem._attributes[key] = value;
		}

		if (deep)
		{
			lock (this._childNodes)
			{
				foreach (var node in this._childNodes)
				{
					var childNode = node.Clone();
					elem._childNodes.Add(childNode);
					childNode._parent = elem;
				}
			}
		}

		return elem;
	}

	public override Element Clone()
		=> Clone(true);

	static readonly XmlFormatting s_StartTagFormatting = XmlFormatting.None with { WriteEndDocumentOnClose = false };

	public string StartTag()
	{
		var sb = new StringBuilder();

		using (var writer = Xml.CreateWriter(sb, s_StartTagFormatting))
			WriteToInternal(writer, s_StartTagFormatting, false, false);

		return sb.ToString();
	}

	public string EndTag()
		=> string.Concat("</", XmlConvert.EncodeName(TagName), '>');

	internal void WriteToInternal(XmlWriter writer, in XmlFormatting formatting, bool includeChildren = true, bool writeEndTag = true)
	{
		string skipAttrName = "xmlns";

		if (this._prefix == null)
			writer.WriteStartElement(this._localName, this.GetNamespace(this._prefix));
		else
		{
			writer.WriteStartElement(this._prefix, this._localName, this.GetNamespace(this._prefix));
			skipAttrName = $"xmlns:{this._prefix}";
		}

		lock (this._attributes)
		{
			foreach (var (name, value) in this._attributes)
			{
				if (skipAttrName == name)
					continue;

				var info = Xml.ExtractQualifiedName(name);

				if (!info.HasPrefix)
					writer.WriteAttributeString(name, value);
				else
					writer.WriteAttributeString(info.LocalName, info.Prefix switch
					{
						"xml" => Namespaces.Xml,
						"xmlns" => Namespaces.Xmlns,
						_ => this.GetNamespace(info.Prefix) ?? string.Empty
					}, value);
			}
		}

		if (includeChildren)
		{
			lock (this._childNodes)
			{
				foreach (var node in this._childNodes)
					node.WriteTo(writer, formatting);
			}
		}

		if (writeEndTag)
			writer.WriteEndElement();
	}

	public override void WriteTo(XmlWriter writer, in XmlFormatting formatting)
		=> WriteToInternal(writer, formatting);

	public string LocalName
	{
		get => this._localName;
		set
		{
			Require.NotNullOrWhiteSpace(value, nameof(this.LocalName));
			this._localName = value;
		}
	}

	public string Prefix
	{
		get => this._prefix;
		set => this._prefix = string.IsNullOrWhiteSpace(value) ? null : value;
	}

	public string TagName
		=> this._prefix == null ? this._localName : string.Concat(this._prefix, ':', this._localName);

	public Node FirstNode
	{
		get
		{
			lock (this._childNodes)
				return this._childNodes.Count > 0 ? this._childNodes[0] : null;
		}
	}

	public Node LastNode
	{
		get
		{
			lock (this._childNodes)
				return this._childNodes.Count > 0 ? this._childNodes[^1] : null;
		}
	}

	public Element FirstChild
	{
		get
		{
			lock (this._childNodes)
			{
				for (int i = 0; i < this._childNodes.Count; i++)
				{
					if (this._childNodes[i] is Element e)
						return e;
				}
			}

			return null;
		}
	}

	public Element LastChild
	{
		get
		{
			lock (this._childNodes)
			{
				for (int i = this._childNodes.Count - 1; i >= 0; i--)
				{
					if (this._childNodes[i] is Element e)
						return e;
				}
			}

			return null;
		}
	}

	public virtual void AddChild(Node n)
	{
		if (n == null)
			return;

		if (n.Parent != null)
			n = n.Clone();

		lock (this._childNodes)
			this._childNodes.Add(n);

		n._parent = this;
	}

	public virtual void RemoveChild(Node n)
	{
		if (n._parent != this)
			return;

		lock (this._childNodes)
		{
			n._parent = null;

			this._childNodes.Remove(n);

			if (n is Element elem)
			{
				var prefix = elem.Prefix;

				if (prefix != null)
					elem.SetNamespace(prefix, this.GetNamespace(prefix));
				else
					elem.SetNamespace(this.GetNamespace());
			}
		}
	}

	public string GetNamespace(string? prefix = default)
	{
		string result;

		if (string.IsNullOrWhiteSpace(prefix))
			result = this.GetAttribute("xmlns");
		else
			result = this.GetAttribute($"xmlns:{prefix}");

		if (result != null)
			return result;

		return this._parent?.GetNamespace(prefix);
	}

	public void SetNamespace(string uri)
		=> this.SetAttribute("xmlns", uri);

	public void SetNamespace(string prefix, string uri)
	{
		Require.NotNullOrWhiteSpace(prefix);
		this.SetAttribute($"xmlns:{prefix}", uri);
	}

	public string? GetAttribute(string name)
	{
		Require.NotNullOrWhiteSpace(name);

		string value;

		lock (this._attributes)
			this._attributes.TryGetValue(name, out value);

		return value;
	}

	public IReadOnlyDictionary<string, string> Attributes()
	{
		KeyValuePair<string, string>[] result;

		lock (_attributes)
			result = _attributes.ToArray();

		return result.ToDictionary(x => x.Key, x => x.Value);
	}

	public Element SetAttribute(string name, object? value)
	{
		Require.NotNullOrWhiteSpace(name);

		lock (this._attributes)
		{
			if (value == null)
				this._attributes.Remove(name);
			else
				this._attributes[name] = Convert.ToString(value, CultureInfo.InvariantCulture);
		}

		return this;
	}

	public void RemoveAllChildNodes()
	{
		lock (_childNodes)
		{
			foreach (var item in _childNodes)
			{
				item._parent = null;

				if (item is Element elem)
				{
					var prefix = elem.Prefix;

					if (prefix != null)
						elem.SetNamespace(prefix, this.GetNamespace(prefix));
					else
						elem.SetNamespace(this.GetNamespace());
				}
			}

			_childNodes.Clear();
		}
	}

	public void RemoveAllAttributes()
	{
		lock (_attributes)
			_attributes.Clear();
	}

	public void Clear()
	{
		RemoveAllChildNodes();
		RemoveAllAttributes();
	}

	public Element RemoveAttribute(string name)
	{
		Require.NotNullOrWhiteSpace(name);

		lock (this._attributes)
			this._attributes.Remove(name);

		return this;
	}

	public bool HasAttribute(string name)
	{
		Require.NotNullOrWhiteSpace(name);

		lock (this._attributes)
			return this._attributes.ContainsKey(name);
	}

	public IEnumerable<Element> Children()
	{
		var result = new List<Element>();

		lock (this._childNodes)
		{
			foreach (var node in _childNodes)
			{
				if (node is Element e)
					result.Add(e);
			}
		}

		return result;
	}

	public IEnumerable<T> Children<T>()
		=> this.Children().OfType<T>();

	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static bool SelectElementPredicate(Element e, string tagName, string? namespaceURI)
	{
		var @namespace = !string.IsNullOrWhiteSpace(e.Prefix)
			? e.GetNamespace(e.Prefix)
			: e.Namespace;

		return tagName == e.TagName && (
			namespaceURI == null || @namespace == namespaceURI
		);
	}

	public IEnumerable<Element> Children(string tagName, string? namespaceURI = default)
	{
		Require.NotNull(tagName);
		return this.Children(e => SelectElementPredicate(e, tagName, namespaceURI));
	}

	public IEnumerable<Element> Children(Func<Element, bool> predicate)
	{
		Require.NotNull(predicate);
		return Children().Where(predicate);
	}

	public Element? Child(string tagName, string? namespaceURI = default)
	{
		Require.NotNull(tagName);
		return Children(tagName, namespaceURI).FirstOrDefault();
	}

	public T Child<T>() where T : Element
		=> this.Children().OfType<T>().FirstOrDefault();

	public Element Child(Func<Element, bool> predicate)
	{
		Require.NotNull(predicate);

		foreach (var element in this.Children())
		{
			if (predicate(element))
				return element;
		}

		return null;
	}

	public string? GetTag(string tagName, string? namespaceURI = default)
	{
		Require.NotNullOrWhiteSpace(tagName);
		return this.Child(tagName, namespaceURI)?.Value;
	}

	public void SetTag(string tagName)
	{
		Require.NotNullOrWhiteSpace(tagName);
		this.AddChild(new Element(tagName));
	}

	public void SetTag(string tagName, string? namespaceURI = default, object? value = default)
	{
		Require.NotNullOrWhiteSpace(tagName);

		var elem = new Element(tagName, namespaceURI);

		if (value != null)
			elem.Value = Convert.ToString(value, CultureInfo.InvariantCulture);

		this.AddChild(elem);
	}

	public void RemoveTag(string tagName, string? namespaceURI = default)
	{
		Require.NotNullOrWhiteSpace(tagName);
		this.Children(tagName, namespaceURI).ForEach(n => n.Remove());
	}

	public bool HasTag(string tagName, string? namespaceURI = default)
	{
		Require.NotNullOrWhiteSpace(tagName);
		return this.Child(tagName, namespaceURI) is not null;
	}

	public void ReplaceWith(Element other)
	{
		Require.NotNull(other);

		var parent = this.Parent;
		this.Remove();
		parent?.AddChild(other);
	}

	public void ReplaceFrom(ref Element other)
	{
		Require.NotNull(other);
		other.Remove();
		other = this;
	}
}
