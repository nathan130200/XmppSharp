using System;

namespace XmppSharp.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class XmppElementAttribute : Attribute
    {
        public string Name { get; }
        public string Xmlns { get; }

        public XmppElementAttribute(string name, string xmlns)
        {
            this.Name = name;
            this.Xmlns = xmlns;
        }
    }
}
