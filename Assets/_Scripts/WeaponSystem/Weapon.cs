using System;
using UnitSystem;
using UnityEngine;

namespace WeaponSystem
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform muzzleTransform;
        [SerializeField] private Bullet bulletPrefab;


        public void Shoot(IBulletTarget target, Action onShoot)
        {
            var newBullet = Instantiate(bulletPrefab);
            newBullet.SetUp(this, target, onShoot);
        }

        public void Equip()
        {
            gameObject.SetActive(true);
        }

        public void UnEquip()
        {
            gameObject.SetActive(false);
        }

        public Vector3 GetMuzzlePosition()
        {
            return muzzleTransform.position;
        }
    }
}