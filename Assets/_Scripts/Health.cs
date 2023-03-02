using System;
using UnityEngine;

public class Health : MonoBehaviour, IProgress
{
    [SerializeField] private int fullHealth = 100;
    
    
    public event Action<ProgressChangedArgs> OnProgressChanged;
    

    public event Action<TakeDamageArgs> OnTakeDamage; 
    public struct TakeDamageArgs
    {
        public int damage;
    }

    public event Action OnDead;
    

    private int m_Health;


    private void Awake()
    {
        RestoreHealth();
    }


    public void Damage(int value)
    {
        SetHealth(m_Health - value);
        OnTakeDamage?.Invoke(new TakeDamageArgs
        {
            damage = value
        });
    }

    public int GetHealth()
    {
        return m_Health;
    }
    

    private void RestoreHealth()
    {
        SetHealth(fullHealth);
    }
    
    private void SetHealth(int value)
    {
        value = Mathf.Max(0, value);
        m_Health = value;
        OnProgressChanged?.Invoke(new ProgressChangedArgs
        {
            minValue = 0f,
            maxValue = fullHealth,
            currentValue = m_Health,
            progressNormalized = Mathf.InverseLerp(0f, fullHealth, value)
        });

        if (m_Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke();
    }
}