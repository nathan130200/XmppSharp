namespace XmppSharp.Dom;

public static class DomExtensions
{
    extension(Element self)
    {
        public Element C(string name, Action<Element>? editor = default)
        {
            return self.C(name, null, editor);
        }

        public Element C(string name, string? ns, Action<Element>? editor = default)
        {
            var el = new Element(name, ns ?? self.Namespace);

            editor?.Invoke(el);

            self.AddChild(el);

            return el;
        }

        public Element Up()
        {
            return self._parent ?? self;
        }

        public Element Root()
        {
            var scope = self;

            while (scope._parent != null)
                scope = scope._parent;

            return scope;
        }
    }
}
