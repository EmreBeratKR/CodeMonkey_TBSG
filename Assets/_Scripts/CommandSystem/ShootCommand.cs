using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;

namespace CommandSystem
{
    public class ShootCommand : BaseCommand
    {
        private const float ShootRange = 4.5f;
        
        
        [SerializeField] private Weapon weapon;


        public static event Action<ShootArgs> OnAnyShoot; 

        public event Action<ShootArgs> OnShoot;
        public struct ShootArgs
        {
            public Unit shooterUnit;
            public Unit shotUnit;
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
            m_UnitToShoot = args.unitToShoot;
            m_State = State.Aim;
            m_Timer = aimTimer;
            StartCommand(onCompleted);
        }

        public override IEnumerator<GridPosition> GetAllValidGridPositions()
        {
            var levelGrid = GetLevelGrid();
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(ShootRange);

            while (allGridPositionsWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionsWithinRange.Current;
                var unit = levelGrid.GetUnitAtGridPosition(gridPosition);
                
                if (!unit) continue;
                
                if (unit.IsInsideTeam(Unit.GetTeamType())) continue;

                yield return gridPosition;
            }
            
            allGridPositionsWithinRange.Dispose();
        }

        public override IEnumerator<(GridPosition, GridVisual.State)> GetAllGridPositionStates()
        {
            var levelGrid = GetLevelGrid();
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(ShootRange);

            while (allGridPositionsWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionsWithinRange.Current;
                var unit = levelGrid.GetUnitAtGridPosition(gridPosition);

                if (!unit)
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue);
                    continue;
                }

                if (unit.IsInsideTeam(Unit.GetTeamType()))
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.Blue);
            }
            
            allGridPositionsWithinRange.Dispose();
        }

        public override string GetName()
        {
            const string commandName = "Shoot";
            return commandName;
        }

        public override float GetBenefitValue(CommandArgs args)
        {
            const float baseBenefitValue = 100f;
            var benefitValue = baseBenefitValue;

            if (!args.unitToShoot)
            {
                var noUnitPenalty = MassiveBenefitPenalty;
                benefitValue -= noUnitPenalty;
            }

            else
            {
                var healthBonus = 25f / args.unitToShoot.GetHealth();
                benefitValue += healthBonus;
            }

            return benefitValue;
        }

        public float GetRange()
        {
            return ShootRange;
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
                        var args = new ShootArgs
                        {
                            shooterUnit = Unit,
                            shotUnit = m_UnitToShoot,
                            impactOffset = GetImpactOffset(Unit, m_UnitToShoot)
                        };
                        OnShoot?.Invoke(args);
                        OnAnyShoot?.Invoke(args);
                        
                        const int damage = 40;
                        m_UnitToShoot.Damage(damage);
                        
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
        
        
        private static Vector3 GetImpactOffset(Unit shooterUnit, Unit shotUnit)
        {
            var directionNormalized = (shooterUnit.GetPosition() - shotUnit.GetPosition())
                .normalized;
            const float offsetDistance = 1f;
            return directionNormalized * offsetDistance;
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