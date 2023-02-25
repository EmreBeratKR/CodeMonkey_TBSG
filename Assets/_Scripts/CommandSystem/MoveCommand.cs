using System;
using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;

namespace CommandSystem
{
    public class MoveCommand : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        
        
        public event Action OnStartMoving;
        public event Action OnStopMoving;
        
        
        private Vector3 m_TargetPosition;
        private bool m_IsMoving;


        private void Start()
        {
            m_TargetPosition = transform.position;
        }

        private void Update()
        {
            MoveTowardsTargetPosition();
            LookTowardsTargetPosition();
        }


        public void Move(Vector3 position)
        {
            m_TargetPosition = position;
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            var validGridPositions = GetAllValidGridPositions();

            while (validGridPositions.MoveNext())
            {
                if (gridPosition != validGridPositions.Current) continue;
                
                validGridPositions.Dispose();
                return true;
            }
            
            validGridPositions.Dispose();
            return false;
        }
        
        public IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            const float maxMoveDistance = 2f;
            
            var levelGrid = GetLevelGrid();
            
            for (var x = 0; x < levelGrid.GetSizeX(); x++)
            {
                for (var y = 0; y < levelGrid.GetSizeY(); y++)
                {
                    for (var z = 0; z < levelGrid.GetSizeZ(); z++)
                    {
                        var gridPosition = new GridPosition(x, y, z);
                    
                        if (!levelGrid.IsValidGridPosition(gridPosition)) continue;
                        
                        var isGreaterThanMaxDistance = GridPosition
                            .Distance(unit.GridPosition, gridPosition) > maxMoveDistance;
                        
                        if (isGreaterThanMaxDistance) continue;

                        yield return gridPosition;
                    }
                }
            }
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

            var direction = m_TargetPosition - transform.position;

            const float rotateSpeed = 25f;
            transform.rotation = Quaternion
                .Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotateSpeed);
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

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}