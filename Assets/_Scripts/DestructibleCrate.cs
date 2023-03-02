using GridSystem;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour, IObstacle, ITakeDamage
{
    [SerializeField] private Health health;


    private GridObject m_GridObject;
    

    private void Awake()
    {
        health.OnDead += OnDead;
    }

    private void OnDestroy()
    {
        health.OnDead -= OnDead;
    }


    private void OnDead()
    {
        m_GridObject.RemoveObstacle(this);
        Destroy(gameObject);
    }
    
    
    public void TakeDamage(int value)
    {
        health.Damage(value);
    }

    public void SetGridObject(GridObject gridObject)
    {
        m_GridObject = gridObject;
    }
}