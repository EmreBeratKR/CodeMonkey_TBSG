using System;
using UnityEngine;

namespace General
{
    public class Sweeper : MonoBehaviour
    {
        [SerializeField] private AnimationCurve sizeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        [SerializeField] private float timeInSeconds = 0.5f;
        [SerializeField] private float delayInSeconds;


        private Action m_OnComplete;
        private float m_Progress;
        private float m_Speed;
        private float m_Timer;
        private bool m_IsActive;


        private void Update()
        {
            if (!m_IsActive) return;

            m_Timer = Mathf.Max(0f, m_Timer - Time.deltaTime);
            
            if (m_Timer > 0f) return;

            m_Progress = Mathf.Clamp01(m_Progress + Time.deltaTime * m_Speed);

            transform.localScale = Vector3.one * sizeCurve.Evaluate(m_Progress);
            
            if (m_Progress < 1f) return;
            
            m_OnComplete?.Invoke();
            Destroy(gameObject);
        }
        

        public void Sweep(Action onSweepComplete = null)
        {
            if (m_IsActive) return;
            
            m_Timer = delayInSeconds;
            m_Speed = 1f / timeInSeconds;
            m_Progress = 0f;
            m_IsActive = true;
            m_OnComplete = onSweepComplete;
        }
    }
}