using General;
using GridSystem;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour, IObstacle, ITakeDamage
{
    [SerializeField] private Explosion destroyed;
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
        Explode();
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


    private void Explode()
    {
        destroyed.gameObject.SetActive(true);
        destroyed.transform.parent = null;
        var explosionPosition = transform.position;
        destroyed.Explode(explosionPosition);

        var sweepers = destroyed.GetComponentsInChildren<Sweeper>(true);

        foreach (var sweeper in sweepers)
        {
            sweeper.Sweep();
        }
    }
}