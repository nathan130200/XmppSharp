using System;
using System.Collections.Concurrent;
using System.Reflection;
using XmppSharp.Attributes;
using XmppSharp.Xml.Dom;

namespace XmppSharp.Factory
{
    /// <summary>
    /// Represents collection of elements metadata to help in parsing and converting raw XML to POCO.
    /// </summary>
    public class ElementFactory
    {
        /// <summary>
        /// Globally registred types.
        /// </summary>
        static ConcurrentDictionary<string, ElementType> Types;

        static ElementFactory()
        {
            Types = new ConcurrentDictionary<string, ElementType>();
        }

        /// <summary>
        /// Register elements from given type via reflection.
        /// </summary>
        /// <remarks>
        /// The type must be annotated with <see cref="XmppElementAttribute"/> attribute.
        /// </remarks>
        /// <param name="type">Type to register.</param>
        public static void RegisterType(Type type)
        {
            if (ElementType.TryExtractElementTypesFor(type, out System.Collections.Generic.IEnumerable<ElementType> items))
            {
                foreach (ElementType item in items)
                    Types[item.QualifiedName] = item;
            }
        }

        /// <summary>
        /// Register elements from given type.
        /// </summary>
        /// <param name="type">Type to register.</param>
        /// <param name="name">Tag name</param>
        /// <param name="xmlns">Xml namespace</param>
        public static void RegisterType(Type type, string name, string xmlns)
        {
            ElementType xte = new(type, name, xmlns);
            Types[xte.QualifiedName] = xte;
        }

        /// <summary>
        /// Register elements from given type via reflection.
        /// </summary>
        /// <typeparam name="TElement">Type to register.</typeparam>
        /// /// <remarks>
        /// The type must be annotated with <see cref="XmppElementAttribute"/> attribute.
        /// </remarks>
        public static void RegisterType<TElement>() where TElement : Element
            => RegisterType(typeof(TElement));

        /// <summary>
        /// Register element from given params.
        /// </summary>
        /// <typeparam name="TElement">Type to register.</typeparam>
        /// <param name="name">Tag name</param>
        /// <param name="xmlns">Xml namespace</param>
        public static void RegisterType<TElement>(string name, string xmlns)
            => RegisterType(typeof(TElement), name, xmlns);

        /// <summary>
        /// Register all possible elements from entire assembly.
        /// </summary>
        /// <param name="assembly">Assembly that scan will be performed</param>
        public static void RegisterTypes(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
                RegisterType(type);
        }
    }
}
