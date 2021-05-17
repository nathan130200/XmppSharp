namespace XmppSharp.Xml.Dom
{
    /// <summary>
    /// Represents base class of XML.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Type of current node implementation.
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        /// Owner node.
        /// </summary>
        public Node Parent { get; protected internal set; }

        public Node(NodeType type)
            => this.Type = type;

        /// <summary>
        /// Remove current node from parent.
        /// </summary>
        public virtual void Remove()
        {

        }
    }
}
