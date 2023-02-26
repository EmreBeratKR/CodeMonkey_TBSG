using System;
using System.Collections.Generic;
using GridSystem;
using UnitSystem;
using UnityEngine;

namespace CommandSystem
{
    public class ShootCommand : BaseCommand
    {
        [SerializeField] private Weapon weapon;


        public event Action<ShootArgs> OnShoot;
        public struct ShootArgs
        {
            public Unit shooterUnit;
            public Unit shotUnit;
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
            const float maxShootDistance = 5f;

            var levelGrid = GetLevelGrid();
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(maxShootDistance);

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
        
        public override string GetName()
        {
            const string commandName = "Shoot";
            return commandName;
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
                        OnShoot?.Invoke(new ShootArgs
                        {
                            shooterUnit = Unit,
                            shotUnit = m_UnitToShoot
                        });
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