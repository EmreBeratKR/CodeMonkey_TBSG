using System;
using UnitSystem;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private Bullet bulletPrefab;


    public void Shoot(Unit unit, Action onShoot)
    {
        var newBullet = Instantiate(bulletPrefab);
        newBullet.SetUp(this, unit, onShoot);
    }

    public Vector3 GetMuzzlePosition()
    {
        return muzzleTransform.position;
    }
}