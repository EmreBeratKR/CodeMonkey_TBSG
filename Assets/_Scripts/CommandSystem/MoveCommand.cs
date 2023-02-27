using System;
using System.Collections.Generic;
using GridSystem;
using UI;
using UnityEngine;

namespace CommandSystem
{
    public class MoveCommand : BaseCommand
    {
        private const float MoveRange = 3f;
        
        
        private Vector3 m_PositionToMove;


        private void Start()
        {
            m_PositionToMove = transform.position;
        }

        private void Update()
        {
            if (!isActive) return;
            
            MoveTowardsTargetPosition();
            LookTowardsPosition(m_PositionToMove);
        }


        public override void Execute(CommandArgs args, Action onCompleted)
        {
            m_PositionToMove = args.positionToMove;
            StartCommand(onCompleted);
        }
        
        public override IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            var levelGrid = GetLevelGrid();
            var allGridPositionWithinRange = GetAllGridPositionWithinRange(MoveRange);

            while (allGridPositionWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionWithinRange.Current;

                if (levelGrid.HasAnyUnitAtGridPosition(gridPosition)) continue;

                yield return gridPosition;
            }
            
            allGridPositionWithinRange.Dispose();
        }

        public override IEnumerator<(GridPosition, GridVisual.State)> GetAllGridPositionStates()
        {
            var levelGrid = GetLevelGrid();
            var allGridPositionWithinRange = GetAllGridPositionWithinRange(MoveRange);

            while (allGridPositionWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionWithinRange.Current;

                if (levelGrid.HasAnyUnitAtGridPosition(gridPosition))
                {
                    yield return (gridPosition, GridVisual.State.Orange);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.White);
            }
            
            allGridPositionWithinRange.Dispose();
        }

        public override string GetName()
        {
            const string commandName = "Move";
            return commandName;
        }

        public override float GetBenefitValue(CommandArgs args)
        {
            const float rewardPerNonTeamUnit = 10f;

            if (!Unit.TryGetCommand(out ShootCommand shootCommand)) return 0f;

            var levelGrid = GetLevelGrid();
            var gridPosition = levelGrid.GetGridPosition(args.positionToMove);
            var gridPositions = GetAllGridPositionWithinRange(gridPosition, shootCommand.GetRange());

            var nearByNonTeamUnit = 0;
            
            while (gridPositions.MoveNext())
            {
                var unitAtGridPosition = levelGrid.GetUnitAtGridPosition(gridPositions.Current);
                
                if (!unitAtGridPosition) continue;
                
                if (Unit.IsInsideTeam(unitAtGridPosition.GetTeamType())) continue;

                nearByNonTeamUnit += 1;
            }
            
            gridPositions.Dispose();

            var allUnits = UnitManager.GetAllUnits();
            var totalSqrDistanceToNonTeamUnits = 0f;

            foreach (var unit in allUnits)
            {
                if (Unit.IsInsideTeam(unit.GetTeamType())) continue;

                var sqrDistance = GridPosition.SqrDistance(unit.GridPosition, gridPosition);
                totalSqrDistanceToNonTeamUnits += sqrDistance;
            }

            return rewardPerNonTeamUnit * nearByNonTeamUnit + 1000f / totalSqrDistanceToNonTeamUnits;
        }


        private void MoveTowardsTargetPosition()
        {
            if (HasReachedToTargetPosition())
            {
                CompleteCommand();
                return;
            }

            var directionNormalized = (m_PositionToMove - transform.position).normalized;
            const float moveSpeed = 4f;
            var motion = directionNormalized * (Time.deltaTime * moveSpeed);
            transform.position += motion;
        }

        private bool HasReachedToTargetPosition()
        {
            const float maxSqrDistanceError = 0.001f;
            var sqrDistanceToTarget = Vector3.SqrMagnitude(m_PositionToMove - transform.position);

            return sqrDistanceToTarget <= maxSqrDistanceError;
        }
    }
}