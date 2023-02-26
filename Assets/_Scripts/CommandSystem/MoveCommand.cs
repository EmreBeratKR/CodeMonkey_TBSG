using System;
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
            if (!isActive) return;
            
            MoveTowardsTargetPosition();
            LookTowardsTargetPosition();
        }


        public override void Execute(CommandArgs args, Action onCompleted)
        {
            m_TargetPosition = args.positionToMove;
            StartCommand(onCompleted);
        }
        
        public override IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            const float maxMoveDistance = 2f;

            var maxDistanceInt = Mathf.FloorToInt(maxMoveDistance);
            var unitGridPosition = Unit.GridPosition;
            var maxGridPosition = unitGridPosition + new GridPosition(1, 0, 1) * maxDistanceInt;
            var minGridPosition = unitGridPosition - new GridPosition(1, 0, 1) * maxDistanceInt;
            
            var levelGrid = GetLevelGrid();

            for (var x = minGridPosition.x; x <= maxGridPosition.x; x++)
            {
                for (var y = maxGridPosition.y; y <= maxGridPosition.y; y++)
                {
                    for (var z = minGridPosition.z; z <= maxGridPosition.z; z++)
                    {
                        var gridPosition = new GridPosition(x, y, z);

                        if (!levelGrid.IsValidGridPosition(gridPosition)) continue;
                        
                        var isGreaterThanMaxDistance = GridPosition
                            .Distance(unitGridPosition, gridPosition) > maxMoveDistance;
                        
                        if (isGreaterThanMaxDistance) continue;

                        yield return gridPosition;
                    }
                }
            }
        }
        
        public override string GetName()
        {
            const string commandName = "Move";
            return commandName;
        }


        private void MoveTowardsTargetPosition()
        {
            if (HasReachedToTargetPosition())
            {
                CompleteCommand();
                return;
            }

            var directionNormalized = (m_TargetPosition - transform.position).normalized;
            const float moveSpeed = 4f;
            var motion = directionNormalized * (Time.deltaTime * moveSpeed);
            transform.position += motion;
        }

        private void LookTowardsTargetPosition()
        {
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
        

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}