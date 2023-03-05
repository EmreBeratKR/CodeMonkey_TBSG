using UnityEngine;
using UnityEngine.Events;

namespace General
{
    public class Trigger : MonoBehaviour
    {
        public UnityEvent<ITriggerSource> onTriggerEnter;
        
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ITriggerSource triggerSource))
            {
                onTriggerEnter?.Invoke(triggerSource);
            }
        }
    }
}