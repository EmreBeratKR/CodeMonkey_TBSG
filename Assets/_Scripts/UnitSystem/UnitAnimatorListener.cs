using System;
using UnityEngine;

namespace UnitSystem
{
    public class UnitAnimatorListener : MonoBehaviour
    {
        public event Action OnMeleeHit;
        public event Action OnMeleeComplete;
        
        
        private void Animator_OnMeleeHit()
        {
            OnMeleeHit?.Invoke();
        }
        
        private void Animator_OnMeleeComplete()
        {
            OnMeleeComplete?.Invoke();
        }
    }
}