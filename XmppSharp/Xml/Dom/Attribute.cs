using System.Collections.Generic;
using System.Xml;

namespace XmppSharp.Xml.Dom
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> node.
    /// </summary>
    public class Attribute : Node
    {
        internal static readonly IComparer<Attribute> DefaultComparer;

        static Attribute()
        {
            DefaultComparer = Comparer<Attribute>.Create((left, right) =>
            {
                if (IsXmlnsWithoutPrefix(left) && !IsNamespaceDeclaration(right))
                    return -1;

                if (IsXmlnsWithoutPrefix(left) && IsXmlnsWithPrefix(right))
                    return -1;

                if (IsXmlnsWithPrefix(left) && !IsNamespaceDeclaration(right))
                    return -1;

                return left.QualifiedName.CompareTo(right.QualifiedName);
            });

            bool IsNamespaceDeclaration(Attribute attr)
                => !IsXmlnsWithoutPrefix(attr) || !IsXmlnsWithPrefix(attr);

            bool IsXmlnsWithoutPrefix(Attribute attr)
                => attr.Name.Equals("xmlns") && string.IsNullOrEmpty(attr.Prefix);

            bool IsXmlnsWithPrefix(Attribute attr)
                => !string.IsNullOrEmpty(attr.Prefix) && attr.Prefix.Equals("xmlns");
        }

        private string _name;
        private string _prefix;
        private string _value;
        private string _qualifiedName;

        /// <summary>
        /// Attribute name.
        /// </summary>
        public string Name
        {
            get => this._name;
            set
            {
                this._name = XmlConvert.EncodeLocalName(value);
                this.BuildQName();
            }
        }

        /// <summary>
        /// Attribute prefix.
        /// </summary>
        public string Prefix
        {
            get => this._prefix;
            set
            {
                this._prefix = XmlConvert.EncodeLocalName(value);
                this.BuildQName();
            }
        }

        /// <summary>
        /// Attribute value.
        /// </summary>
        public string Value
        {
            get => Util.UnescapeXml(this._value);
            set => this._value = Util.EscapeXml(value);
        }

        public Attribute(string name, string value) : base(NodeType.Attribute)
        {
            this.Name = name;
            this.Value = value;
        }

        public Attribute(string name, string value, string prefix = default) : this(name, value)
        {
            this.Prefix = prefix;
        }

        /// <summary>
        /// Qualified name that represents this attribute.
        /// </summary>
        public string QualifiedName
            => this._qualifiedName;

        void BuildQName()
            => this._qualifiedName = BuildQualifiedName(this.Name, this.Prefix);

        internal static string BuildQualifiedName(string name, string prefix = default)
        {
            if (!string.IsNullOrEmpty(prefix))
                name = string.Concat(prefix, ':', name);

            return name;
        }

        public override void Remove()
        {
            if (this.Parent is Element e)
                e.RemoveAttribute(this);
        }
    }
}
