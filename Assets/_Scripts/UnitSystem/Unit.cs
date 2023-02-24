using System;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour
    {
        public event Action OnStartMoving;
        public event Action OnStopMoving;
    
    
        private GridPosition m_GridPosition;
        private Vector3 m_TargetPosition;
        private bool m_IsMoving;


        private void Start()
        {
            m_TargetPosition = transform.position;
            m_GridPosition = GetGridPosition();
            GetLevelGrid().AddUnitToGridPosition(this, m_GridPosition);
        }


        private void Update()
        {
            MoveTowardsTargetPosition();
            LookTowardsTargetPosition();
            ApplyGridPosition();
        }


        public void Move(Vector3 position)
        {
            m_TargetPosition = position;
        }
    

        private void MoveTowardsTargetPosition()
        {
            if (HasReachedToTargetPosition())
            {
                if (m_IsMoving) StopMoving();
            
                return;
            }
        
            if (!m_IsMoving) StartMoving();
        
            var directionNormalized = (m_TargetPosition - transform.position).normalized;
            const float moveSpeed = 4f;
            var motion = directionNormalized * (Time.deltaTime * moveSpeed);
            transform.position += motion;
        }

        private void LookTowardsTargetPosition()
        {
            if (!m_IsMoving) return;

            var directionNormalized = m_TargetPosition - transform.position;

            const float rotateSpeed = 25f;
            transform.forward = Vector3
                .Lerp(transform.forward, directionNormalized.normalized, Time.deltaTime * rotateSpeed);
        }

        private bool HasReachedToTargetPosition()
        {
            const float maxSqrDistanceError = 0.0001f;
            var sqrDistanceToTarget = Vector3.SqrMagnitude(m_TargetPosition - transform.position);

            return sqrDistanceToTarget <= maxSqrDistanceError;
        }

        private void StartMoving()
        {
            m_IsMoving = true;
            OnStartMoving?.Invoke();
        }

        private void StopMoving()
        {
            m_IsMoving = false;
            OnStopMoving?.Invoke();
        }

        private GridPosition GetGridPosition()
        {
            return GetLevelGrid()
                .GetGridPosition(transform.position);
        }

        private void ApplyGridPosition()
        {
            var gridPosition = GetGridPosition();
            
            if (gridPosition == m_GridPosition) return;

            var levelGrid = GetLevelGrid();
            levelGrid.RemoveUnitFromGridPosition(this, m_GridPosition);
            levelGrid.AddUnitToGridPosition(this, gridPosition);
            m_GridPosition = gridPosition;
        }
        

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}
