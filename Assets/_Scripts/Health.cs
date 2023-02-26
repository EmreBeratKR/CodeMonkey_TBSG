using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    private const int FullHealth = 100;
    
    
    public event Action<HealthChangedArgs> OnHealthChanged;
    public struct HealthChangedArgs
    {
        public int health;
        public float healthNormalized;
    }

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
    

    private void RestoreHealth()
    {
        SetHealth(FullHealth);
    }
    
    private void SetHealth(int value)
    {
        value = Mathf.Max(0, value);
        m_Health = value;
        OnHealthChanged?.Invoke(new HealthChangedArgs
        {
            health = value,
            healthNormalized = Mathf.InverseLerp(0f, FullHealth, value)
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