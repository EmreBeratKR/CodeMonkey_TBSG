using System;

namespace AI
{
    public class RepeatUntilNode : Node
    {
        private readonly Func<bool> m_Condition;


        public RepeatUntilNode(Func<bool> condition)
        {
            m_Condition = condition;
        }


        public override NodeState Tick()
        {
            if (m_Condition())
            {
                return NodeState.Succeed;
            }

            TickCurrentChildNode();
            return NodeState.Running;
        }
    }
}