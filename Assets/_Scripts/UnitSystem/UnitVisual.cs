using CommandSystem;
using UnityEngine;

namespace UnitSystem
{
    public class UnitVisual : MonoBehaviour
    {
        private static readonly int IsMovingID = Animator.StringToHash("IsMoving");
        private static readonly int ShootID = Animator.StringToHash("Shoot");


        [SerializeField] private Unit unit;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject selectedVisual;


        private void Start()
        {
            var moveCommand = unit.GetCommand<MoveCommand>();

            if (moveCommand)
            {
                moveCommand.OnStart += OnStartMoving;
                moveCommand.OnComplete += OnCompleteMoving;
            }

            var shootCommand = unit.GetCommand<ShootCommand>();

            if (shootCommand)
            {
                shootCommand.OnShoot += OnShoot;
            }

            UnitCommander.OnSelectedUnitChanged += OnSelectedUnitChanged;
        }

        private void OnDestroy()
        {
            var moveCommand = unit.GetCommand<MoveCommand>();

            if (moveCommand)
            {
                moveCommand.OnStart -= OnStartMoving;
                moveCommand.OnComplete -= OnCompleteMoving;
            }
            
            var shootCommand = unit.GetCommand<ShootCommand>();

            if (shootCommand)
            {
                shootCommand.OnShoot -= OnShoot;
            }

            UnitCommander.OnSelectedUnitChanged -= OnSelectedUnitChanged;
        }

    
        private void OnStartMoving()
        {
            SetIsMoving(true);
        }
    
        private void OnCompleteMoving()
        {
            SetIsMoving(false);
        }
        
        private void OnShoot(ShootCommand.ShootArgs args)
        {
            TriggerShoot();
        }
    
        private void OnSelectedUnitChanged(UnitCommander.SelectedUnitChangedArgs args)
        {
            var isSelected = args.unit == unit;
            SetActiveSelectedVisual(isSelected);
        }


        private void SetIsMoving(bool value)
        {
            animator.SetBool(IsMovingID, value);
        }

        private void TriggerShoot()
        {
            animator.SetTrigger(ShootID);
        }

        private void SetActiveSelectedVisual(bool value)
        {
            selectedVisual.SetActive(value);
        }
    }
}