using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;

namespace CommandSystem
{
    public abstract class BaseCommand : MonoBehaviour
    {
        public event Action OnStart;
        public event Action OnComplete;


        protected Unit Unit { get; private set; }
        
        
        protected bool isActive;


        protected virtual void Awake()
        {
            Unit = GetComponent<Unit>();
        }


        public abstract void Execute(CommandArgs args);
        public abstract IEnumerator<GridPosition> GetAllValidGridPositions();
        
        public virtual string GetName()
        {
            return GetType().Name;
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


        protected void InvokeOnStart()
        {
            OnStart?.Invoke();
        }

        protected void InvokeOnComplete()
        {
            OnComplete?.Invoke();
        }
    }
}