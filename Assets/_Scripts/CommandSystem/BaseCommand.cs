using System;
using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
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
        public abstract IEnumerator<GridPosition> GetAllValidGridPositions();
        public abstract IEnumerator<(GridPosition, GridVisual.State)> GetAllGridPositionStates();

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
            
            var levelGrid = GetLevelGrid();

            for (var x = minGridPosition.x; x <= maxGridPosition.x; x++)
            {
                for (var y = maxGridPosition.y; y <= maxGridPosition.y; y++)
                {
                    for (var z = minGridPosition.z; z <= maxGridPosition.z; z++)
                    {
                        var gridPosition = new GridPosition(x, y, z);

                        if (!levelGrid.IsValidGridPosition(gridPosition)) continue;

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
            var levelGrid = GetLevelGrid();
            LookTowardsPosition(levelGrid.GetWorldPosition(unit.GridPosition));
        }


        protected static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}