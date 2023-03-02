using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;
using WeaponSystem;

namespace CommandSystem
{
    public class MeleeCommand : BaseCommand
    {
        private const float MeleeRange = 1f;
        
        
        [SerializeField] private Melee melee;
        [SerializeField] private bool allowFriendlyFire;


        public static event Action<ShootCommand.AttackArgs> OnAnyUse;
        public event Action<ShootCommand.AttackArgs> OnUse;

        public static event Action OnAnyHit;
        public event Action OnHit;


        private Unit m_TargetUnit;
        

        private void Update()
        {
            if (!isActive) return;
            
            LookTowardsUnit(m_TargetUnit);
        }


        private void OnMeleeHit()
        {
            m_TargetUnit.TakeDamage(100);
            OnHit?.Invoke();
            OnAnyHit?.Invoke();
        }

        private void OnMeleeComplete()
        {
            CompleteCommand();
        }
        

        public override void Execute(CommandArgs args, Action onCompleted)
        {
            m_TargetUnit = args.unit;
            Use();
            StartCommand(onCompleted);
        }

        public override IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates()
        {
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(MeleeRange);

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

                if (!allowFriendlyFire && IsFriendlyFire(unit))
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue, CommandStatus.FriendlyFire);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.Blue, CommandStatus.Ok);
            }
            
            allGridPositionsWithinRange.Dispose();
        }

        public override string GetName()
        {
            return melee.name;
        }

        public override int GetRequiredCommandPoint()
        {
            return 2;
        }
        

        protected override void OnSelected()
        {
            melee.Equip();
        }

        protected override void OnDeselected()
        {
            melee.UnEquip();
        }


        private void Use()
        {
            var args = new ShootCommand.AttackArgs
            {
                attackerUnit = Unit,
                attackedUnit = m_TargetUnit,
                impactOffset = GetImpactOffset(Unit, m_TargetUnit)
            };
            melee.Use(OnMeleeHit, OnMeleeComplete);
            OnAnyUse?.Invoke(args);
            OnUse?.Invoke(args);
        }
    }
}