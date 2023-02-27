using System.Collections.Generic;

namespace AI
{
    public abstract class Node
    {
        private readonly List<Node> m_ChildNodes = new();
        private Node m_ParentNode;
        private int m_ChildNodeIndex;


        public virtual NodeState Tick()
        {
            return NodeState.Running;
        }


        public void AddNode(Node node)
        {
            node.m_ParentNode = this;
            m_ChildNodes.Add(node);
        }


        protected NodeState TickCurrentChildNode()
        {
            return GetCurrentChildNode().Tick();
        }
        
        protected bool MoveNextChildNode()
        {
            m_ChildNodeIndex = m_ChildNodeIndex + 1;
            var childNodeCount = m_ChildNodes.Count;
            var hasTraversAllChildNodes = m_ChildNodeIndex >= childNodeCount;

            if (!hasTraversAllChildNodes) return true;

            m_ChildNodeIndex -= childNodeCount;
            return false;
        }


        private Node GetCurrentChildNode()
        {
            return m_ChildNodes[m_ChildNodeIndex];
        }
    }
}