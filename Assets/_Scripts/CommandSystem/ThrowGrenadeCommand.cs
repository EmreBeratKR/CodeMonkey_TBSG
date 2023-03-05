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
        [SerializeField] private bool allowFriendlyFire = true;


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
                Explode();
                CompleteCommand();
            });
            OnShoot?.Invoke();
        }

        public override IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates()
        {
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(ThrowRange);

            while (allGridPositionsWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionsWithinRange.Current;

                if (!HasEnoughCommandPoint())
                {
                    yield return (gridPosition, GridVisual.State.Red, CommandStatus.NotEnoughCommandPoint);
                    continue;
                }
                
                var unit = LevelGrid.GetUnitAtGridPosition(gridPosition);

                if (!allowFriendlyFire && IsFriendlyFire(unit))
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue, CommandStatus.FriendlyFire);
                    continue;
                }

                if (IsBlockedByObstacle(gridPosition, unit))
                {
                    yield return (gridPosition, GridVisual.State.Orange, CommandStatus.Blocked);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.Blue, CommandStatus.Ok);
            }
            
            allGridPositionsWithinRange.Dispose();
        }

        public override string GetName()
        {
            const string commandName = "Grenade";
            return commandName;
        }

        public override int GetRequiredCommandPoint()
        {
            return 2;
        }


        protected override void OnSelected()
        {
            weapon.Equip();
        }

        protected override void OnDeselected()
        {
            weapon.UnEquip();
        }


        private void Explode()
        {
            var origin = m_TargetGridObject.GetWorldPosition();
            const float explosionRadius = 1.5f;
            const int damagePerTarget = 10;
            
            var count = Physics
                .OverlapSphereNonAlloc(origin, explosionRadius, ColliderBuffer, Physics.AllLayers, QueryTriggerInteraction.Collide);

            for (var i = 0; i < count; i++)
            {
                if (!ColliderBuffer[i].TryGetComponent(out ITakeDamage target)) continue;
                
                target.TakeDamage(damagePerTarget);
            }
            
            OnAnyGrenadeExplode?.Invoke();
        }
    }
}