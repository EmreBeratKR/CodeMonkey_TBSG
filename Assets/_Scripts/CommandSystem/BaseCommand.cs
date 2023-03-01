using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;

namespace CommandSystem
{
    public abstract class BaseCommand : MonoBehaviour
    {
        protected const float MassiveBenefitPenalty = 9999f;
        
        private static readonly RaycastHit[] RaycastHitBuffer = new RaycastHit[10];
        
        
        public event Action OnStart;
        public event Action OnComplete;


        public Unit Unit { get; private set; }
        
        
        protected bool isActive;
        
        
        private Action m_OnCompletedCallback;

        
        protected virtual void Awake()
        {
            Unit = GetComponent<Unit>();

            UnitCommander.OnSelectedCommandChanged += UnitCommand_OnSelectedCommandChanged;
        }

        protected virtual void OnDestroy()
        {
            UnitCommander.OnSelectedCommandChanged -= UnitCommand_OnSelectedCommandChanged;
        }


        private void UnitCommand_OnSelectedCommandChanged(UnitCommander.SelectedCommandChangedArgs args)
        {
            if (args.newCommand == this)
            {
                OnSelected();
            }

            if (args.oldCommand == this)
            {
                OnDeselected();
            }
        }
        

        public abstract void Execute(CommandArgs args, Action onCompleted);
        public abstract IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates();

        public virtual string GetName()
        {
            return GetType().Name;
        }

        public virtual int GetRequiredCommandPoint()
        {
            return 1;
        }

        public virtual float GetBenefitValue(CommandArgs args)
        {
            return 0f;
        }
        
        public IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            var states = GetAllGridPositionStates();

            while (states.MoveNext())
            {
                var status = states.Current.Item3;
                
                if (status != CommandStatus.Ok) continue;

                yield return states.Current.Item1;
            }
            
            states.Dispose();
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

        public CommandStatus GetStatusByGridPosition(GridPosition gridPosition)
        {
            var states = GetAllGridPositionStates();

            while (states.MoveNext())
            {
                var currentGridPosition = states.Current.Item1;
                
                if (currentGridPosition != gridPosition) continue;

                states.Dispose();
                return states.Current.Item3;
            }
            
            states.Dispose();
            return CommandStatus.NotFound;
        }
        
        public bool HasEnoughCommandPoint()
        {
            return Unit.HasEnoughCommandPoint(this);
        }


        protected virtual void OnSelected()
        {
            
        }

        protected virtual void OnDeselected()
        {
            
        }
        
        protected void StartCommand(Action onCompleteCallback)
        {
            isActive = true;
            m_OnCompletedCallback = onCompleteCallback;
            OnStart?.Invoke();
        }

        protected void CompleteCommand()
        {
            isActive = false;
            m_OnCompletedCallback?.Invoke();
            OnComplete?.Invoke();
        }

        protected IEnumerator<GridPosition> GetAllGridPositionWithinRange(float range)
        {
            return GetAllGridPositionWithinRange(Unit.GridPosition, range);
        }

        protected IEnumerator<GridPosition> GetAllGridPositionWithinRange(GridPosition center, float range)
        {
            var maxDistanceInt = Mathf.FloorToInt(range);
            var maxGridPosition = center + new GridPosition(1, 0, 1) * maxDistanceInt;
            var minGridPosition = center - new GridPosition(1, 0, 1) * maxDistanceInt;

            for (var x = minGridPosition.x; x <= maxGridPosition.x; x++)
            {
                for (var y = maxGridPosition.y; y <= maxGridPosition.y; y++)
                {
                    for (var z = minGridPosition.z; z <= maxGridPosition.z; z++)
                    {
                        var gridPosition = new GridPosition(x, y, z);

                        if (!LevelGrid.IsValidGridPosition(gridPosition)) continue;

                        var deltaGridPosition = gridPosition - center;
                        var distanceFactor = Mathf.Abs(deltaGridPosition.x) + Mathf.Abs(deltaGridPosition.z);
                        var isTooFarAway = distanceFactor > maxDistanceInt;
                        
                        if (isTooFarAway) continue;

                        yield return gridPosition;
                    }
                }
            }
        }
        
        protected IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllProjectileGridPositionStates(float range)
        {
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(range);

            while (allGridPositionsWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionsWithinRange.Current;

                if (!HasEnoughCommandPoint())
                {
                    yield return (gridPosition, GridVisual.State.Red, CommandStatus.NotEnoughCommandPoint);
                    continue;
                }
                
                var unit = LevelGrid.GetUnitAtGridPosition(gridPosition);

                if (!unit)
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue, CommandStatus.TargetNotFound);
                    continue;
                }

                if (unit.IsInsideTeam(Unit.GetTeamType()))
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue, CommandStatus.FriendlyFire);
                    continue;
                }

                if (IsBlockedByObstacle(unit))
                {
                    yield return (gridPosition, GridVisual.State.Orange, CommandStatus.Blocked);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.Blue, CommandStatus.Ok);
            }
            
            allGridPositionsWithinRange.Dispose();
        }
        
        protected void LookTowardsPosition(Vector3 targetPosition)
        {
            var direction = targetPosition - transform.position;

            const float rotateSpeed = 25f;
            transform.rotation = Quaternion
                .Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotateSpeed);
        }

        protected void LookTowardsUnit(Unit unit)
        {
            LookTowardsPosition(LevelGrid.GetWorldPosition(unit.GridPosition));
        }
        
        protected bool IsBlockedByObstacle(Unit unit)
        {
            var origin = Unit.GetPosition() + Vector3.up;
            var direction = (unit.GetPosition() - Unit.GetPosition()).normalized;
            var distance = Vector3.Distance(unit.GetPosition(), Unit.GetPosition());
            var hitCount = Physics
                .RaycastNonAlloc(origin, direction, RaycastHitBuffer, distance, Physics.AllLayers, QueryTriggerInteraction.Collide);

            for (var i = 0; i < hitCount; i++)
            {
                if (!RaycastHitBuffer[i].collider.TryGetComponent(out IObstacle obstacle)) continue;

                if (obstacle is not Unit unitObstacle) return true;
                
                if (unit != unitObstacle && Unit != unitObstacle) return true;
            }

            return false;
        }
    }
}