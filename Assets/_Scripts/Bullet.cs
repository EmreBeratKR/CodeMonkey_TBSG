using System;
using UnitSystem;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private GameObject hitVfxPrefab;
    
    
    private Weapon m_ShotFrom;
    private Unit m_UnitToHit;
    private Action m_OnShoot;


    private void Update()
    {
        MoveTowardsUnit();
    }


    public void SetUp(Weapon weapon, Unit unit, Action onShoot)
    {
        transform.position = weapon.GetMuzzlePosition();
        m_ShotFrom = weapon;
        m_UnitToHit = unit;
        m_OnShoot = onShoot;
    }


    private void MoveTowardsUnit()
    {
        var position = LevelGrid
            .GetWorldPosition(m_UnitToHit.GridPosition) + m_UnitToHit.GetShootOffset();

        const float moveSpeed = 150f;

        var sqrDistanceBeforeMove = Vector3.SqrMagnitude(position - transform.position);
        
        var directionNormalized = (position - transform.position).normalized;
        transform.position += directionNormalized * (Time.deltaTime * moveSpeed);
        
        var sqrDistanceAfterMove = Vector3.SqrMagnitude(position - transform.position);
        
        if (sqrDistanceAfterMove < sqrDistanceBeforeMove) return;

        transform.position = position;
        DestroySelf();
        Instantiate(hitVfxPrefab, position, Quaternion.identity);
        
        m_OnShoot?.Invoke();
    }
    
    private void DestroySelf()
    {
        Destroy(gameObject);
        trail.transform.parent = null;
        Destroy(trail.gameObject, trail.time);
    }
}