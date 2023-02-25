using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnityEngine;

namespace CommandSystem
{
    public class MoveCommand : BaseCommand
    {
        private Vector3 m_TargetPosition;


        private void Start()
        {
            m_TargetPosition = transform.position;
        }

        private void Update()
        {
            MoveTowardsTargetPosition();
            LookTowardsTargetPosition();
        }


        public override string GetName()
        {
            const string commandName = "Move";
            return commandName;
        }

        public override void Execute(CommandArgs args)
        {
            m_TargetPosition = args.positionToMove;
        }

        public override IEnumerator<GridPosition> GetAllValidGridPositions()
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
                            .Distance(Unit.GridPosition, gridPosition) > maxMoveDistance;
                        
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
                if (isActive) StopMoving();
            
                return;
            }
        
            if (!isActive) StartMoving();
        
            var directionNormalized = (m_TargetPosition - transform.position).normalized;
            const float moveSpeed = 4f;
            var motion = directionNormalized * (Time.deltaTime * moveSpeed);
            transform.position += motion;
        }

        private void LookTowardsTargetPosition()
        {
            if (!isActive) return;

            var direction = m_TargetPosition - transform.position;

            const float rotateSpeed = 25f;
            transform.rotation = Quaternion
                .Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotateSpeed);
        }

        private bool HasReachedToTargetPosition()
        {
            const float maxSqrDistanceError = 0.001f;
            var sqrDistanceToTarget = Vector3.SqrMagnitude(m_TargetPosition - transform.position);

            return sqrDistanceToTarget <= maxSqrDistanceError;
        }
        
        private void StartMoving()
        {
            isActive = true;
            InvokeOnStart();
        }

        private void StopMoving()
        {
            isActive = false;
            InvokeOnComplete();
        }

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}