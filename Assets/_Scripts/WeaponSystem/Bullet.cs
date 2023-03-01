using System;
using UnityEngine;

namespace WeaponSystem
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private GameObject hitVfxPrefab;
        [SerializeField] private AnimationCurve heightCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);
        [SerializeField] private float heightFactor = 1f;
        [SerializeField] private float moveSpeed = 1f;
    
    
        private IBulletTarget m_Target;
        private Vector3 m_ShotPosition;
        private Weapon m_ShotFrom;
        private float m_Progress;
        private float m_Distance;
        private Action m_OnShoot;


        private void Update()
        {
            MoveTowardsUnit();
        }


        public void SetUp(Weapon weapon, IBulletTarget target, Action onShoot)
        {
            m_ShotPosition = weapon.GetMuzzlePosition();
            transform.position = m_ShotPosition;
            m_ShotFrom = weapon;
            m_Target = target;
            m_Progress = 0f;
            m_Distance = Vector3.Distance(m_ShotPosition, GetTargetPosition());
            m_OnShoot = onShoot;
        }


        private void MoveTowardsUnit()
        {
            var deltaProgress = moveSpeed * Time.deltaTime / m_Distance;
            m_Progress = Mathf.Clamp01(m_Progress + deltaProgress);

            if (m_Progress >= 1f)
            {
                OnShot();
                return;
            }

            var position = Vector3.Lerp(m_ShotPosition, GetTargetPosition(), m_Progress);
            position.y += heightCurve.Evaluate(m_Progress) * heightFactor * m_Distance;
            transform.position = position;
        }

        private void OnShot()
        {
            var position = GetTargetPosition();
            
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

        private Vector3 GetTargetPosition()
        {
            return m_Target.GetHitPosition();
        }
    }
}