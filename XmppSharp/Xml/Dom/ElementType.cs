using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using XmppSharp.Attributes;

namespace XmppSharp.Xml.Dom
{
    /// <summary>
    /// Represents metadata of element type registred in library.
    /// <para>This will be helpful when parsing xml and casting to correct classes.</para>
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public sealed class ElementType : IEquatable<ElementType>
    {
        /// <summary>
        /// XML Tag Name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// XML Namespace
        /// </summary>
        public string Xmlns { get; }

        /// <summary>
        /// Computed Qualified Name
        /// </summary>
        public string QualifiedName { get; }

        /// <summary>
        /// Owner Type
        /// </summary>
        public Type Type { get; }

        internal ElementType(Type type, string name, string xmlns = default)
        {
            if (string.IsNullOrEmpty(xmlns))
                xmlns = string.Empty;

            this.Name = name;
            this.Xmlns = xmlns;
            this.Type = type;
            this.QualifiedName = BuildQualifiedName(this.Name, this.Xmlns);
        }

        public override string ToString()
          => this.QualifiedName;

        public override int GetHashCode()
            => HashCode.Combine(this.Name, this.Xmlns);

        public override bool Equals(object obj)
            => obj is ElementType other && this.Equals(other);

        public bool Equals(ElementType other)
        {
            if (other is null)
                return false;

            if (other == this)
                return true;

            return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase)
              && string.Equals(this.Xmlns, other.Xmlns, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Build qualified name for element type key.
        /// </summary>
        /// <param name="tag">XML Tag</param>
        /// <param name="ns">XML Namespace</param>
        /// <returns>Valid qualified name for element type registration</returns>
        internal static string BuildQualifiedName(string tag, string ns = default)
        {
            if (!string.IsNullOrEmpty(ns))
                tag = string.Concat(ns, ':', tag);

            return tag;
        }

        /// <summary>
        /// Try extract valid element types from given reflected type.
        /// </summary>
        /// <param name="type">Type to scan</param>
        /// <param name="result">Result containing all possible element types</param>
        /// <returns>
        /// <see langword="true"/> if have found element tyles. Otherwise <see langword="false" />
        /// </returns>
        public static bool TryExtractElementTypesFor(Type type, out IEnumerable<ElementType> result)
        {
            result = type.GetCustomAttributes<XmppElementAttribute>()
                .Select(xa => new ElementType(type, xa.Name, xa.Xmlns));

            return result.Any();
        }
    }
}
