using UnityEngine;

namespace AI
{
    public abstract class BehaviourTree : MonoBehaviour
    {
        private Node m_RootNode;


        protected abstract Node SetupTree();


        protected virtual void Awake()
        {
            m_RootNode = SetupTree();
        }

        protected virtual void Update()
        {
            m_RootNode?.Tick();
        }
    }
}