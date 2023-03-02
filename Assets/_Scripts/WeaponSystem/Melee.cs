using System;
using UnitSystem;
using UnityEngine;

namespace WeaponSystem
{
    public class Melee : MonoBehaviour
    {
        [SerializeField] private UnitAnimatorListener animatorListener;
        
        
        private Action m_OnHit;
        private Action m_OnComplete;


        private void Awake()
        {
            animatorListener.OnMeleeHit += Animator_OnMeleeHit;
            animatorListener.OnMeleeComplete += Animator_OnMeleeComplete;
        }

        private void OnDestroy()
        {
            animatorListener.OnMeleeHit -= Animator_OnMeleeHit;
            animatorListener.OnMeleeComplete -= Animator_OnMeleeComplete;
        }
        
        
        private void Animator_OnMeleeHit()
        {
            m_OnHit();
        }

        private void Animator_OnMeleeComplete()
        {
            m_OnComplete?.Invoke();
        }


        public void Use(Action onHit, Action onComplete)
        {
            m_OnHit = onHit;
            m_OnComplete = onComplete;
        }
        
        public void Equip()
        {
            gameObject.SetActive(true);
        }

        public void UnEquip()
        {
            gameObject.SetActive(false);
        }
    }
}