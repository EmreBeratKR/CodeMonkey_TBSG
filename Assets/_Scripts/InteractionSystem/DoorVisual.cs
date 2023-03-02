using System;
using UnityEngine;

namespace InteractionSystem
{
    public class DoorVisual : MonoBehaviour
    {
        private static readonly int IsOpenID = Animator.StringToHash("IsOpen");
        private static readonly int SpeedID = Animator.StringToHash("Speed");


        [SerializeField] private Door door;
        [SerializeField] private Animator animator;


        public event Action OnComplete;
        

        private void Awake()
        {
            SetSpeed(door.GetSpeed());

            door.OnToggle += OnToggled;
        }

        private void OnDestroy()
        {
            door.OnToggle -= OnToggled;
        }


        private void OnToggled(bool value)
        {
            SetIsOpen(value);
        }

        private void Animator_OnComplete()
        {
            OnComplete?.Invoke();
        }
        

        private void SetSpeed(float value)
        {
            animator.SetFloat(SpeedID ,value);
        }

        private void SetIsOpen(bool value)
        {
            animator.SetBool(IsOpenID, value);
        }
    }
}