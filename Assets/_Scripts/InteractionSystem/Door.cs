using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;
using UnityEngine.Events;

namespace InteractionSystem
{
    public class Door : MonoBehaviour, IInteractable, IObstacle
    {
        [SerializeField] private DoorVisual visual;
        [SerializeField] private float toggleDuration = 0.5f;


        public UnityEvent onOpen, onClose;

        public event Action<bool> OnToggle;


        private List<GridObject> m_GridObjects = new();
        private Action m_OnComplete;
        private bool m_IsOpen;


        private void Awake()
        {
            visual.OnComplete += OnCompleted;
        }

        private void OnDestroy()
        {
            visual.OnComplete -= OnCompleted;
        }


        private void OnCompleted()
        {
            m_OnComplete?.Invoke();
        }
        

        public void Interact(Unit interactor, Action onComplete)
        {
            m_OnComplete = onComplete;
            SetIsOpen(!m_IsOpen);
        }

        public bool IsValid()
        {
            foreach (var gridObject in m_GridObjects)
            {
                if (gridObject.HasAnyUnit()) return false;
            }

            return true;
        }
        
        public void SetGridObject(GridObject gridObject)
        {
            m_GridObjects.Add(gridObject);
        }

        public float GetSpeed()
        {
            return 1f / toggleDuration;
        }
        

        private void SetIsOpen(bool value)
        {
            m_IsOpen = value;

            if (value)
            {
                foreach (var gridObject in m_GridObjects)
                {
                    gridObject.RemoveObstacle(this);
                }
            }

            else
            {
                foreach (var gridObject in m_GridObjects)
                {
                    gridObject.AddObstacle(this);
                }
            }
            
            OnToggle?.Invoke(value);

            if (value)
            {
                onOpen?.Invoke();
            }
            else
            {
                onClose?.Invoke();
            }
        }
    }
}