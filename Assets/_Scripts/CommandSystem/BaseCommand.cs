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
        
        
        public event Action OnStart;
        public event Action OnComplete;


        public Unit Unit { get; private set; }
        
        
        protected bool isActive;
        
        
        private Action m_OnCompletedCallback;

        
        protected virtual void Awake()
        {
            Unit = GetComponent<Unit>();
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
    }
}