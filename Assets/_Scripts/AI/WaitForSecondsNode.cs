using UnityEngine;

namespace AI
{
    public class WaitForSecondsNode : Node
    {
        private readonly float m_SecondsToWait;
        private float m_SecondsLeftToWait;
        
        
        public WaitForSecondsNode(float seconds)
        {
            m_SecondsToWait = seconds;
            m_SecondsLeftToWait = seconds;
        }


        public override NodeState Tick()
        {
            TickTimer();

            if (!IsTimerDone()) return NodeState.Running;
            
            m_SecondsLeftToWait = m_SecondsToWait;
            return NodeState.Succeed;

        }


        private void TickTimer()
        {
            m_SecondsLeftToWait = Mathf.Max(0f, m_SecondsLeftToWait - Time.deltaTime);
        }

        private bool IsTimerDone()
        {
            return m_SecondsLeftToWait <= 0f;
        }
    }
}