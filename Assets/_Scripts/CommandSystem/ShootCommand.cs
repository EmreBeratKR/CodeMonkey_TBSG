using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;
using WeaponSystem;

namespace CommandSystem
{
    public class ShootCommand : BaseCommand
    {
        private const float ShootRange = 7f;


        [SerializeField] private Weapon weapon;
        [SerializeField] private bool allowFriendlyFire;


        public static event Action<AttackArgs> OnAnyShoot; 

        public event Action<AttackArgs> OnShoot;
        public struct AttackArgs
        {
            public Unit attackerUnit;
            public Unit attackedUnit;
            public Vector3 impactOffset;
        }
        
        
        private Unit m_UnitToShoot;
        private State m_State;
        private float m_Timer;
        
        
        private void Update()
        {
            if (!isActive) return;
            
            TickTimer();
            UpdateState();
        }


        public override void Execute(CommandArgs args, Action onCompleted)
        {
            const float aimTimer = 0.5f;
            m_UnitToShoot = args.unit;
            m_State = State.Aim;
            m_Timer = aimTimer;
            weapon.Equip();
            StartCommand(onCompleted);
        }

        public override IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates()
        {
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(ShootRange);

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

                if (IsBlockedByObstacle(unit))
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
            return weapon.name;
        }

        public override float GetBenefitValue(CommandArgs args)
        {
            const float baseBenefitValue = 500f;
            var benefitValue = baseBenefitValue;

            if (!args.unit)
            {
                var noUnitPenalty = MassiveBenefitPenalty;
                benefitValue -= noUnitPenalty;
            }

            else
            {
                var healthBonus = 50f / args.unit.GetHealth();
                benefitValue += healthBonus;
            }

            return benefitValue;
        }

        public float GetRange()
        {
            return ShootRange;
        }
        
        
        protected override void OnSelected()
        {
            weapon.Equip();
        }

        protected override void OnDeselected()
        {
            weapon.UnEquip();
        }


        private void UpdateState()
        {
            switch (m_State)
            {
                case State.Aim:
                    LookTowardsUnit(m_UnitToShoot);
                    break;
                
                case State.Shoot:
                    break;
                
                case State.UnArm:
                    break;
            }
        }
        
        private void OnTimerDone()
        {
            switch (m_State)
            {
                case State.Aim:
                    const float shootTimer = 99999f;
                    m_State = State.Shoot;
                    m_Timer = shootTimer;
                    weapon.Shoot(m_UnitToShoot, () =>
                    {
                        var args = new AttackArgs
                        {
                            attackerUnit = Unit,
                            attackedUnit = m_UnitToShoot,
                            impactOffset = GetImpactOffset(Unit, m_UnitToShoot)
                        };
                        OnShoot?.Invoke(args);
                        OnAnyShoot?.Invoke(args);
                        
                        const int damage = 10;
                        m_UnitToShoot.TakeDamage(damage);
                        
                        m_Timer = 0f;
                    });
                    break;
                
                case State.Shoot:
                    const float unArmTimer = 0.5f;
                    m_State = State.UnArm;
                    m_Timer = unArmTimer;
                    break;
                
                case State.UnArm:
                    m_State = State.None;
                    CompleteCommand();
                    break;
            }
        }
        
        private void TickTimer()
        {
            m_Timer -= Time.deltaTime;
            
            if (m_Timer > 0f) return;
            
            OnTimerDone();
        }
        


        private enum State
        {
            None,
            Aim,
            Shoot,
            UnArm
        }
    }
}