using CommandSystem;
using UnityEngine;

namespace UnitSystem
{
    public class UnitVisual : MonoBehaviour
    {
        private static readonly int IsMovingID = Animator.StringToHash("IsMoving");
        private static readonly int ShootID = Animator.StringToHash("Shoot");
        private static readonly int SingleShootID = Animator.StringToHash("SingleShoot");
        private static readonly int SlashID = Animator.StringToHash("Slash");


        [SerializeField] private Unit unit;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject selectedVisual;


        private void Start()
        {
            if (unit.TryGetCommand(out MoveCommand moveCommand))
            {
                moveCommand.OnStart += OnStartMoving;
                moveCommand.OnComplete += OnCompleteMoving;
            }

            if (unit.TryGetCommand(out ShootCommand shootCommand))
            {
                shootCommand.OnShoot += ShootCommand_OnShoot;
            }
            
            if (unit.TryGetComponent(out ThrowGrenadeCommand throwGrenadeCommand))
            {
                throwGrenadeCommand.OnShoot += ThrowGrenadeCommand_OnShoot;
            }

            if (unit.TryGetCommand(out MeleeCommand meleeCommand))
            {
                meleeCommand.OnUse += MeleeCommand_OnUse;
            }

            UnitCommander.OnSelectedUnitChanged += OnSelectedUnitChanged;
        }

        private void OnDestroy()
        {
            if (unit.TryGetCommand(out MoveCommand moveCommand))
            {
                moveCommand.OnStart -= OnStartMoving;
                moveCommand.OnComplete -= OnCompleteMoving;
            }

            if (unit.TryGetCommand(out ShootCommand shootCommand))
            {
                shootCommand.OnShoot -= ShootCommand_OnShoot;
            }

            if (unit.TryGetComponent(out ThrowGrenadeCommand throwGrenadeCommand))
            {
                throwGrenadeCommand.OnShoot -= ThrowGrenadeCommand_OnShoot;
            }
            
            if (unit.TryGetCommand(out MeleeCommand meleeCommand))
            {
                meleeCommand.OnUse -= MeleeCommand_OnUse;
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
        
        private void ShootCommand_OnShoot(ShootCommand.AttackArgs args)
        {
            TriggerShoot();
        }

        private void ThrowGrenadeCommand_OnShoot()
        {
            TriggerSingleShoot();
        }
        
        private void MeleeCommand_OnUse(ShootCommand.AttackArgs args)
        {
            TriggerSlash();
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

        private void TriggerSingleShoot()
        {
            animator.SetTrigger(SingleShootID);
        }

        private void TriggerSlash()
        {
            animator.SetTrigger(SlashID);
        }

        private void SetActiveSelectedVisual(bool value)
        {
            selectedVisual.SetActive(value);
        }
    }
}