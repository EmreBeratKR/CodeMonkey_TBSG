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


        public Unit Unit { get; private set; }
        
        
        protected bool isActive;
        
        
        private Action m_OnCompletedCallback;

        
        protected virtual void Awake()
        {
            Unit = GetComponent<Unit>();
        }


        public abstract void Execute(CommandArgs args, Action onCompleted);
        public abstract IEnumerator<GridPosition> GetAllValidGridPositions();
        
        public virtual string GetName()
        {
            return GetType().Name;
        }

        public virtual int GetRequiredCommandPoint()
        {
            return 1;
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
    }
}