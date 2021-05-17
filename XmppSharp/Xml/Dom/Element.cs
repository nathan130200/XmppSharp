using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace XmppSharp.Xml.Dom
{
    /// <summary>
    /// Represents an <see cref="Element"/> node.
    /// <para>
    /// Elements can hold attributes, texts, and children nodes.
    /// </para>
    /// </summary>
    [DebuggerDisplay("{StartTag,nq}")]
    public class Element : Node
    {
        private string _name;
        private string _prefix;
        private string _value;

        /// <summary>
        /// Element name.
        /// </summary>
        public string Name
        {
            get => this._name;
            set => this._name = XmlConvert.EncodeLocalName(value);
        }

        /// <summary>
        /// Element prefix.
        /// </summary>
        public string Prefix
        {
            get => this._prefix;
            set => this._prefix = XmlConvert.EncodeLocalName(value);
        }

        /// <summary>
        /// Element text content.
        /// </summary>
        public string Value
        {
            get => Util.UnescapeXml(this._value);
            set => this._value = Util.EscapeXml(value);
        }

        /// <summary>
        /// Determins the root element of tree.
        /// </summary>
        public Element Root
        {
            get
            {
                var temp = this.Parent as Element;

                while (!temp.IsRootElement)
                    temp = temp.Parent as Element;

                return temp;
            }
        }

        /// <summary>
        /// Determins if current element is root of entire tree.
        /// </summary>
        public bool IsRootElement
            => this.Parent is null;

        private List<Attribute> _attributes;
        private List<Element> _children;

        /// <summary>
        /// Attributes
        /// </summary>
        public IReadOnlyList<Attribute> Attributes
        {
            get
            {
                Attribute[] temp;

                lock (this._attributes)
                    temp = this._attributes.ToArray();

                return temp;
            }
        }

        /// <summary>
        /// Children elements
        /// </summary>
        public IReadOnlyList<Element> Children
        {
            get
            {
                Element[] temp;

                lock (this._children)
                    temp = this._children.ToArray();

                return temp;
            }
        }

        public Element(string name) : base(NodeType.Element)
        {
            this._children = new List<Element>();
            this._attributes = new List<Attribute>();
            this.Name = name;
        }

        public Element(string name, string xmlns = default) : this(name)
        {
            this.SetDefaultNamespace(xmlns);
        }

        public Element(string name, string xmlns = default, string value = default) : this(name, xmlns)
        {
            this.Value = value;
        }

        /// <summary>
        /// Set default xml namespace declaration.
        /// </summary>
        /// <param name="value">Namespace URI</param>
        public void SetDefaultNamespace(string value)
            => this.SetNamespace(value);

        /// <summary>
        /// Set element namespace.
        /// </summary>
        /// <param name="value">Namespace URI</param>
        /// <param name="prefix">Well formed namespace prefix</param>
        public void SetNamespace(string value, string prefix = default)
        {
            string key;

            if (string.IsNullOrEmpty(prefix)) key = Attribute.BuildQualifiedName("xmlns"); // xmlns
            else key = Attribute.BuildQualifiedName(prefix, "xmlns"); // xmlns:whatever

            var attr = this.Attributes.FirstOrDefault(x => x.QualifiedName.Equals(key));

            if (attr != null)
                attr.Value = value;
            else
            {
                string ns, nm = default;

                if (string.IsNullOrWhiteSpace(prefix)) // xmlns
                    ns = "xmlns";
                else // xmlns:whatever
                {
                    ns = prefix;
                    nm = "xmlns";
                }

                attr = new Attribute(ns, value, nm);

                lock (this._attributes)
                {
                    this._attributes.Add(attr);
                    attr.Parent = this;
                }
            }
        }

        public string StartTag
        {
            get
            {
                var part = this.Name;

                if (!string.IsNullOrEmpty(this.Prefix))
                    part = string.Concat(this.Prefix, ':', part);

                var sb = new StringBuilder(string.Concat('<', part));
                var attrs = this.Attributes
                    .OrderBy(x => x, Attribute.DefaultComparer)
                    .ToArray();

                if (this.Attributes.Any())
                {
                    sb.Append(' ');

                    for (var i = 0; i < attrs.Length; i++)
                    {
                        var attr = attrs[i];
                        sb.AppendFormat("{0}=\"{1}\"", attr.QualifiedName, attr.Value);

                        if (i < attrs.Length - 1)
                            sb.Append(' ');
                    }
                }

                if (!this.Attributes.Any() && !this.Children.Any()) // <example/>
                    sb.Append("/ ");

                return sb.Append('>').ToString();
            }
        }

        public string EndTag
        {
            get
            {
                var raw = this.Name;

                if (!string.IsNullOrEmpty(this.Prefix))
                    raw = string.Concat(this.Prefix, ':', raw);

                return raw;
            }
        }

        /// <summary>
        /// Set attribute.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="value">Attribute value</param>
        /// <returns>Instance of current created attribute.</returns>
        public Attribute SetAttribute(string name, string value)
            => this.SetAttribute(name, default, value);

        /// <summary>
        /// Set attribute.
        /// </summary>
        /// <param name="name">Attribute name</param>
        /// <param name="prefix">Attribute prefix</param>
        /// <param name="value">Attribute value</param>
        /// <returns>Instance of current created attribute.</returns>
        public Attribute SetAttribute(string name, string prefix, string value)
        {
            var key = Attribute.BuildQualifiedName(name, prefix);
            var attr = this.Attributes.FirstOrDefault(xa => xa.QualifiedName.Equals(key));

            if (attr != null)
                attr.Value = value;
            else
            {
                attr = new Attribute(name, value, prefix);

                lock (this._attributes)
                {
                    this._attributes.Add(attr);
                    attr.Parent = this;
                }
            }

            return attr;
        }

        /// <summary>
        /// Find an attribute.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <returns>Instance of attribute owned by this element or <see langword="null"/> if no attribute matching was found.</returns>
        public Attribute FindAttribute(string name)
            => this.Attributes.FirstOrDefault(x => x.QualifiedName.Equals(name));

        /// <summary>
        /// Find an attribute.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="prefix">Attribute prefix.</param>
        /// <returns>Instance of attribute owned by this element or <see langword="null"/> if no attribute matching was found.</returns>
        public Attribute FindAttribute(string name, string prefix)
            => this.Attributes.FirstOrDefault(x => x.QualifiedName.Equals(Attribute.BuildQualifiedName(name, prefix)));

        public void AddAttribute(Attribute a)
        {
            if (a.Parent == this)
                return;

            if (a.Parent != null)
                a.Remove();

            lock (this._attributes)
            {
                this._attributes.Add(a);
                a.Parent = this;
            }
        }

        public void RemoveAttribute(Attribute a)
        {
            if (a.Parent != this)
                return;

            lock (this._attributes)
            {
                this._attributes.Remove(a);
                a.Parent = null;
            }
        }

        public Element AddChild(Element e)
        {
            if (e.Parent == this)
                return e;

            e.Remove();

            lock (this._children)
            {
                this._children.Add(e);
                e.Parent = this;
            }

            return e;
        }

        public Element AddChild(string name, string xmlns = default, string value = default)
            => this.AddChild(new Element(name, xmlns, value));

        public void RemoveChild(Element e)
        {
            if (e.Parent != this)
                return;

            lock (this._children)
                this._children.Remove(e);

            e.Parent = null;
        }

        public override void Remove()
        {
            if (this.Parent is Element p)
            {
                p.RemoveChild(this);
                this.Parent = null;
            }
        }

        public override string ToString()
            => this.ToString(Formatting.None);

        /// <summary>
        /// Returns full XML string that represents this element.
        /// </summary>
        /// <param name="formatting">Determins if formatting will be used or not</param>
        /// <returns></returns>
        public string ToString(Formatting formatting)
        {
            // TODO: 
            return base.ToString();
        }

        ~Element()
        {
            if (this._attributes != null)
            {
                lock (this._attributes)
                    this._attributes.Clear();
            }

            if (this._children != null)
            {
                lock (this._children)
                    this._children.Clear();
            }

            this.Remove();
        }
    }
}
