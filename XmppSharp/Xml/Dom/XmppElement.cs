using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using XmppSharp.Abstractions;

namespace XmppSharp.Dom;

public class XmppElement : XmppNode
{
	private List<XmppNode> _children = [];
	private Dictionary<string, string> _namespaces = [];
	private Dictionary<string, string> _attributes = [];

	internal XmlNameInfo _elementName;

	public XmppElement(string tagName, string? uri = null, object? value = null)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tagName);

		_elementName = new(tagName);

		if (uri != null)
			SetNamespace(_elementName.Prefix, uri);

		SetValue(value);
	}

	public string TagName
	{
		get => _elementName.ToString();
		internal set => _elementName = new(value);
	}

	public string LocalName
		=> _elementName.LocalName;

	public string? Prefix
		=> _elementName.Prefix;

	#region Add/remove child nodes

	public void AddChild(XmppNode? node)
	{
		if (node is null)
			return;

		if (node._parent != null)
			node = node.Clone();

		lock (_children)
		{
			_children.Add(node);
			node._parent = this;
		}
	}

	public void RemoveChild(XmppNode? node)
	{
		if (node?._parent != this)
			return;

		lock (_children)
		{
			if (_children == null)
				return;

			_children.Remove(node);

			node._parent = null;
		}
	}

	#endregion

	#region Select nodes helper

	protected IEnumerable<T> SelectNodesOfType<T>(Func<T, bool>? match = null, bool first = false)
	{
		lock (_children)
		{
			foreach (var entry in _children)
			{
				if (entry is not T node)
					continue;

				if (match == null || match(node))
				{
					yield return node;

					if (first)
						yield break;
				}
			}
		}
	}

	protected IEnumerable<XmppNode> SelectNodes(Func<XmppNode, bool>? match = null, bool first = false)
	{
		lock (_children)
		{
			foreach (var node in _children)
			{
				if (match == null || match(node))
				{
					yield return node;

					if (first)
						yield break;
				}
			}
		}
	}

	#endregion

	#region Child nodes accessor.

	public IEnumerable<XmppNode> Nodes()
	{
		lock (_children)
		{
			foreach (var node in _children)
				yield return node;
		}
	}

	public IEnumerable<XmppElement> Elements()
	{
		lock (_children)
		{
			foreach (var node in _children)
			{
				if (node is XmppElement element)
					yield return element;
			}
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining), EditorBrowsable(EditorBrowsableState.Never)]
	public bool Is(string tagName, string? xmlns)
		=> TagName == tagName && (xmlns == null || Namespace == xmlns);

	public IEnumerable<XmppElement> Elements(string tagName, string? xmlns = null)
		=> SelectNodesOfType<XmppElement>(x => x.Is(tagName, xmlns));

	public XmppElement? Element(string tagName, string? xmlns = null)
		=> SelectNodesOfType<XmppElement>(x => x.Is(tagName, xmlns), true).FirstOrDefault();

	public IEnumerable<XmppElement> Elements(Func<XmppElement, bool>? match)
		=> SelectNodesOfType(match);

	public XmppElement? Element(Func<XmppElement, bool> match)
		=> SelectNodesOfType(match).FirstOrDefault();

	public TElement? Element<TElement>() where TElement : XmppElement
		=> SelectNodesOfType<TElement>(first: true).FirstOrDefault();

	public IEnumerable<TElement> Elements<TElement>() where TElement : XmppElement
		=> SelectNodesOfType<TElement>();

	public IEnumerable<TElement> Elements<TElement>(Func<TElement, bool> match) where TElement : XmppElement
		=> SelectNodesOfType(match);

	#endregion

	#region Set text content

	public void SetValue(object? value)
		=> InnerText = Convert.ToString(value, CultureInfo.InvariantCulture);

	public IEnumerable<XmppNode> DescendantNodes()
	{
		List<XmppNode> result = [];
		BuildDescendantNodesList(this, result);
		return result;
	}

	public IEnumerable<XmppNode> DescendantNodesAndSelf()
	{
		List<XmppNode> result = [this];
		BuildDescendantNodesList(this, result);
		return result;
	}

	static void BuildDescendantNodesList(XmppElement context, List<XmppNode> result)
	{
		foreach (var node in context.Nodes())
		{
			result.Add(node);

			if (node is XmppElement e)
				BuildDescendantNodesList(e, result);
		}
	}

	static void BuildDescendantNodesListOfType<T>(XmppElement context, List<T> result) where T : XmppNode
	{
		foreach (var node in context.Nodes())
		{
			if (node is T self)
				result.Add(self);

			if (node is XmppElement e)
				BuildDescendantNodesListOfType(e, result);
		}
	}

	[MaybeNull]
	public string? InnerText
	{
		[return: NotNull]
		get
		{
			var items = from node in DescendantNodes()
							.OfType<IContentNode>()
						select node.Value;

			if (items.Any())
				return string.Concat(items);

			return string.Empty;
		}
		set
		{
			RemoveNodes();

			if (value != null)
				AddChild(new XmppText(value));
		}
	}

	#endregion

	#region Remove helpers

	void UnsafeRemoveAttributes()
	{
		_attributes.Clear();
	}

	void UnsafeRemoveNodes()
	{
		foreach (var node in _children)
			node._parent = null;

		_children.Clear();
	}

	public void RemoveAttributes()
	{
		lock (_attributes)
		{
			UnsafeRemoveAttributes();
		}
	}

	public void RemoveNodes()
	{
		lock (_children)
		{
			UnsafeRemoveNodes();
		}
	}

	public void RemoveAll()
	{
		RemoveAttributes();
		RemoveNodes();
	}

	#endregion

	#region Namespace lookup

	public string Namespace
	{
		get => GetNamespace(Prefix) ?? string.Empty;
		set => SetNamespace(Prefix, value);
	}

	public void SetNamespace(string value) => SetNamespace(string.Empty, value);

	public void SetNamespace(string? prefix, string? value)
	{
		ArgumentNullException.ThrowIfNull(value);

		prefix ??= string.Empty;

		if (!string.IsNullOrWhiteSpace(prefix))
			ArgumentException.ThrowIfNullOrWhiteSpace(value);

		lock (_namespaces)
			_namespaces[prefix] = value;
	}

	public void RemoveNamespace(string? prefix)
	{
		prefix ??= string.Empty;

		lock (_namespaces)
			_namespaces?.Remove(prefix);
	}

	public string? GetNamespace(string? prefix)
	{
		if (prefix == "xml")
			return Namespaces.Xml;

		if (prefix == "xmlns")
			return Namespaces.Xmlns;

		if (LookupNamespace(prefix) is string result)
			return result;

		return default;
	}

	public string? GetNamespaceInScope(string? prefix)
	{
		prefix ??= string.Empty;

		string? result = null;

		lock (_namespaces)
			_namespaces.TryGetValue(prefix, out result);

		return result;
	}

	string? LookupNamespace(string? prefix)
	{
		var scope = this;

		while (scope != null)
		{
			var result = scope.GetNamespaceInScope(prefix);

			if (result != null)
				return result;

			scope = scope._parent;
		}

		return null;
	}

	#endregion

	#region Attributes acessor

	public string? GetAttribute(string name, string? defaultValue = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		lock (_attributes)
			return _attributes.GetValueOrDefault(name, defaultValue!);
	}

	public T GetAttribute<T>(string name, T defaultValue) where T : IParsable<T>
	{
		if (T.TryParse(GetAttribute(name), CultureInfo.InvariantCulture, out var result))
			return result;

		return defaultValue;
	}

	public T? GetAttribute<T>(string name) where T : struct, IParsable<T>
	{
		if (T.TryParse(GetAttribute(name), CultureInfo.InvariantCulture, out var result))
			return result;

		return null;
	}

	public bool HasAttribute(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		lock (_attributes)
			return _attributes.ContainsKey(name);
	}

	public bool RemoveAttribute(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		lock (_attributes)
			return _attributes.Remove(name);
	}

	public void SetAttribute(string name, object? value)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		lock (_attributes)
		{
			if (value is null)
				_attributes.Remove(name);
			else
			{
				var rawValue = Convert.ToString(value, CultureInfo.InvariantCulture);
				_attributes[name] = rawValue ?? string.Empty;
			}
		}
	}

	#endregion

	#region Simple tag access

	public bool HasTag(string name, string? xmlns = default)
		=> Element(name, xmlns) != null;

	public string? GetTag(string name, string? xmlns = default)
		=> Element(name, xmlns)?.InnerText;

	public void RemoveTag(string name, string? xmlns = default)
		=> Element(name, xmlns)?.Remove();

	public void SetTag(string name, string? xmlns = default, object? value = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name);

		var element = Element(name, xmlns);

		if (element != null)
			element.SetValue(value);
		else
		{
			element = XmppElementFactory.CreateElement(name, xmlns);
			AddChild(element);
			element.SetValue(value);
		}
	}

	public XmppElement C(string name, string? xmlns = default, object? value = default)
	{
		var element = XmppElementFactory.CreateElement(name, xmlns);
		element.SetValue(value);
		AddChild(element);
		return element;
	}

	public bool IsRootElement => _parent is null;

	[return: MaybeNull]
	public XmppElement Up() => Parent;

	#endregion

	#region Overrides

	public override XmppNode Clone()
	{
		var result = XmppElementFactory.CreateElement(TagName, Namespace);

		lock (this)
		{
			lock (_namespaces)
			{
				foreach (var (prefix, uri) in _namespaces)
					result._namespaces[prefix] = uri;
			}

			lock (_attributes)
			{
				foreach (var (key, value) in _attributes)
					result._attributes[key] = value;
			}

			lock (_children)
			{
				foreach (var node in _children)
				{
					var temp = node.Clone();
					result._children.Add(temp);
					temp._parent = result;
				}
			}
		}

		return result;
	}

	protected void WriteStartElement(XmlWriter writer)
	{
		writer.WriteStartElement(_elementName.Prefix, _elementName.LocalName, GetNamespace(_elementName.Prefix));

		if (_namespaces != null)
		{
			foreach (var (prefix, uri) in _namespaces)
			{
				if (string.IsNullOrEmpty(prefix))
					writer.WriteAttributeString("xmlns", uri);
				else
					writer.WriteAttributeString(prefix, Namespaces.Xmlns, uri);
			}
		}

		if (_attributes != null)
		{
			foreach (var (key, value) in _attributes)
			{
				var attrName = new XmlNameInfo(key);

				if (!attrName.HasPrefix)
					writer.WriteAttributeString(attrName.LocalName, value);
				else
					writer.WriteAttributeString(attrName.LocalName, GetNamespace(attrName.Prefix), value);
			}
		}
	}

	protected void WriteContent(XmlWriter writer)
	{
		if (_children != null)
		{
			foreach (var node in _children)
				node.WriteTo(writer);
		}
	}

	public override void WriteTo(XmlWriter writer)
	{
		lock (this)
		{
			WriteStartElement(writer);
			WriteContent(writer);
			writer.WriteEndElement();
		}
	}

	public sealed override string ToString() => ToString(false);

	protected static XmlWriter CreateXmlWriter(StringBuilder output, bool indented)
	{
		var settings = new XmlWriterSettings
		{
			Indent = indented,
			IndentChars = indented ? XmlWriterOptions.IndentChars : string.Empty,
			CheckCharacters = XmlWriterOptions.CheckCharacters,
			ConformanceLevel = XmlWriterOptions.ConformanceLevel,
			NamespaceHandling = XmlWriterOptions.NamespaceHandling,
			Encoding = XmlWriterOptions.OutputEncoding,
			OmitXmlDeclaration = XmlWriterOptions.OmitXmlDeclaration,
			NewLineChars = XmlWriterOptions.NewLineChars,
			NewLineOnAttributes = XmlWriterOptions.NewLineOnAttributes,
			WriteEndDocumentOnClose = false
		};

		return XmlWriter.Create(output);
	}

	public virtual string ToString(bool indented)
	{
		var sb = new StringBuilder();

		using (var writer = CreateXmlWriter(sb, indented))
			WriteTo(writer);

		return sb.ToString();
	}

	#endregion
}