using System;
using System.Collections.Generic;
using GridSystem;
using UnityEngine;
using WeaponSystem;

namespace CommandSystem
{
    public class ThrowGrenadeCommand : BaseCommand
    {
        private const float ThrowRange = 7f;


        [SerializeField] private Weapon weapon;


        public static event Action OnAnyGrenadeExplode;
        public event Action OnShoot;
        
        
        private GridObject m_TargetGridObject;


        private void Update()
        {
            if (!isActive) return;
            
            LookTowardsPosition(m_TargetGridObject.GetWorldPosition());
        }


        public override void Execute(CommandArgs args, Action onCompleted)
        {
            m_TargetGridObject = args.gridObject;
            weapon.Equip();
            StartCommand(onCompleted);
            weapon.Shoot(m_TargetGridObject, () =>
            {
                OnAnyGrenadeExplode?.Invoke();
                CompleteCommand();
            });
            OnShoot?.Invoke();
        }

        public override IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates()
        {
            return GetAllProjectileGridPositionStates(ThrowRange);
        }

        public override string GetName()
        {
            const string commandName = "Grenade";
            return commandName;
        }


        protected override void OnSelected()
        {
            weapon.Equip();
        }

        protected override void OnDeselected()
        {
            weapon.UnEquip();
        }
    }
}