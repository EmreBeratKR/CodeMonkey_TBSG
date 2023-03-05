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
        private const float MoveRange = 5f;
        
        
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
            m_Path = Pathfinding.GetPath(Unit.GridPosition, args.gridPosition);
            m_PathIndex = 1;
            StartCommand(onCompleted);
        }

        public override IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates()
        {
            var allGridPositionWithinRange = GetAllGridPositionWithinRange(MoveRange);

            while (allGridPositionWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionWithinRange.Current;

                if (gridPosition == Unit.GridPosition)
                {
                    yield return (gridPosition, GridVisual.State.Clear, CommandStatus.BadSelection);
                    continue;
                }

                if (!HasEnoughCommandPoint())
                {
                    yield return (gridPosition, GridVisual.State.Red, CommandStatus.NotEnoughCommandPoint);
                    continue;
                }
                
                if (LevelGrid.HasAnyObstacleAtGridPosition(gridPosition))
                {
                    yield return (gridPosition, GridVisual.State.Orange, CommandStatus.Blocked);
                    continue;
                }
                
                if (!Pathfinding.HasValidPath(Unit.GridPosition, gridPosition, out var cost))
                {
                    yield return (gridPosition, GridVisual.State.Clear, CommandStatus.InvalidPath);
                    continue;
                }
                
                var outOfRange = cost / Pathfinding.GetCostMultiplier() > MoveRange;

                if (outOfRange)
                {
                    yield return (gridPosition, GridVisual.State.Red, CommandStatus.TooFarAway);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.White, CommandStatus.Ok);
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
            
            var gridPosition = args.gridPosition;
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