using System;
using System.Collections.Generic;
using GridSystem;
using PathfindingSystem;
using UI;
using UnityEngine;

namespace CommandSystem
{
    public class MoveCommand : BaseCommand
    {
        private const float MoveRange = 3f;
        
        
        private IReadOnlyList<Vector3> m_Path;
        private int m_PathIndex;

        
        private void Update()
        {
            if (!isActive) return;

            var position = GetCurrentPathPosition();
            MoveTowardsPosition(position);
            LookTowardsPosition(position);
        }


        public override void Execute(CommandArgs args, Action onCompleted)
        {
            m_Path = Pathfinding.GetPath(Unit.GridPosition, args.gridPositionToMove);
            m_PathIndex = 1;
            StartCommand(onCompleted);
        }
        
        public override IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            var allGridPositionWithinRange = GetAllGridPositionWithinRange(MoveRange);

            while (allGridPositionWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionWithinRange.Current;

                if (LevelGrid.HasAnyUnitAtGridPosition(gridPosition)) continue;
                
                if (!Pathfinding.HasValidPath(Unit.GridPosition, gridPosition, out var cost)) continue;

                var outOfRange = cost / Pathfinding.GetCostMultiplier() > MoveRange; 
                
                if (outOfRange) continue;

                yield return gridPosition;
            }
            
            allGridPositionWithinRange.Dispose();
        }

        public override IEnumerator<(GridPosition, GridVisual.State)> GetAllGridPositionStates()
        {
            var allGridPositionWithinRange = GetAllGridPositionWithinRange(MoveRange);

            while (allGridPositionWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionWithinRange.Current;

                if (LevelGrid.HasAnyUnitAtGridPosition(gridPosition))
                {
                    yield return (gridPosition, GridVisual.State.Orange);
                    continue;
                }
                
                if (!Pathfinding.HasValidPath(Unit.GridPosition, gridPosition, out var cost))
                {
                    yield return (gridPosition, GridVisual.State.Clear);
                    continue;
                }
                
                var outOfRange = cost / Pathfinding.GetCostMultiplier() > MoveRange;

                if (outOfRange)
                {
                    yield return (gridPosition, GridVisual.State.Clear);
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
            
            var gridPosition = args.gridPositionToMove;
            var gridPositions = GetAllGridPositionWithinRange(gridPosition, shootCommand.GetRange());

            var nearByNonTeamUnit = 0;
            
            while (gridPositions.MoveNext())
            {
                var unitAtGridPosition = LevelGrid.GetUnitAtGridPosition(gridPositions.Current);
                
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


        private void MoveTowardsPosition(Vector3 position)
        {
            if (HasReachedToPosition(position))
            {
                m_PathIndex += 1;

                if (IsPathCompleted())
                {
                    CompleteCommand();
                    return;
                }
            }

            var directionNormalized = (position - transform.position).normalized;
            const float moveSpeed = 4f;
            var motion = directionNormalized * (Time.deltaTime * moveSpeed);
            transform.position += motion;
        }

        private Vector3 GetCurrentPathPosition()
        {
            return m_Path[m_PathIndex];
        }

        private bool HasReachedToPosition(Vector3 position)
        {
            const float maxSqrDistanceError = 0.001f;
            var sqrDistanceToTarget = Vector3.SqrMagnitude(position - transform.position);

            return sqrDistanceToTarget <= maxSqrDistanceError;
        }

        private bool IsPathCompleted()
        {
            return m_PathIndex >= m_Path.Count;
        }
    }
}