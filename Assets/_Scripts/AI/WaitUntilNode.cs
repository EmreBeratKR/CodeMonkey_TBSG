using System;

namespace AI
{
    public class WaitUntilNode : Node
    {
        private readonly Func<bool> m_Condition;


        public WaitUntilNode(Func<bool> condition)
        {
            m_Condition = condition;
        }


        public override NodeState Tick()
        {
            if (m_Condition == null) return NodeState.Failure;

            return m_Condition()
                ? NodeState.Succeed
                : NodeState.Running;
        }
    }
}