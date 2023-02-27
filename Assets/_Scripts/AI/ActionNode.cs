using System;

namespace AI
{
    public class ActionNode : Node
    {
        private readonly Func<NodeState> m_Action;


        public ActionNode(Func<NodeState> action)
        {
            m_Action = action;
        }


        public override NodeState Tick()
        {
            if (m_Action == null) return NodeState.Failure;

            return m_Action();
        }
    }
}