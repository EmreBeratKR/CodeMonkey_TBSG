using System;
using System.Collections.Generic;
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour
    {
        private const int MaxCommandPoint = 2;


        [SerializeField] private Ragdoll ragdollPrefab;
        [SerializeField] private Transform rootBone;
        [SerializeField] private Health health;
        [SerializeField] private TeamType teamType;
        

        public static event Action<AnyUnitUsedCommandPointArgs> OnAnyUnitUsedCommandPoint;
        public struct AnyUnitUsedCommandPointArgs
        {
            public Unit unit;
        }
        
        
        public GridPosition GridPosition { get; private set; }
        public int CommandPoint { get; private set; } = MaxCommandPoint;


        private BaseCommand[] m_Commands;


        private void Awake()
        {
            m_Commands = GetComponents<BaseCommand>();

            health.OnDead += OnDead;
            
            TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
        }

        private void Start()
        {
            GridPosition = GetGridPosition();
            GetLevelGrid().AddUnitToGridPosition(this, GridPosition);
        }

        private void OnDestroy()
        {
            health.OnDead -= OnDead;
            
            TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
        }


        private void Update()
        {
            ApplyGridPosition();
        }

        
        private void OnDead()
        {
            var levelGrid = GetLevelGrid();
            levelGrid.RemoveUnitFromGridPosition(this, GridPosition);
            Die();
        }
        
        private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
        {
            RestoreCommandPoints();
        }


        public BaseCommand GetDefaultCommand()
        {
            return m_Commands[0];
        }
        
        public IReadOnlyList<BaseCommand> GetAllCommands()
        {
            return m_Commands;
        }

        public T GetCommand<T>()
            where T : BaseCommand
        {
            foreach (var command in m_Commands)
            {
                if (command is not T commandAsT) continue;

                return commandAsT;
            }

            return null;
        }

        public bool TryUseCommandPoint(BaseCommand command)
        {
            if (!HasEnoughCommandPoint(command)) return false;
            
            UseCommandPoint(command);
            return true;
        }
        
        public bool HasEnoughCommandPoint(BaseCommand command)
        {
            return CommandPoint >= command.GetRequiredCommandPoint();
        }

        public TeamType GetTeamType()
        {
            return teamType;
        }

        public bool IsInsideTeam(TeamType team)
        {
            return teamType == team;
        }

        public Vector3 GetShootOffset()
        {
            return Vector3.up * 1.5f;
        }

        public void Damage(int value)
        {
            health.Damage(value);
        }


        private void UseCommandPoint(BaseCommand command)
        {
            var requiredCommandPoint = command.GetRequiredCommandPoint();
            SetCommandPoint(CommandPoint - requiredCommandPoint);
        }

        private void RestoreCommandPoints()
        {
            SetCommandPoint(MaxCommandPoint);
        }

        private void SetCommandPoint(int value)
        {
            CommandPoint = value;
            OnAnyUnitUsedCommandPoint?.Invoke(new AnyUnitUsedCommandPointArgs
            {
                unit = this
            });
        }
        
        private GridPosition GetGridPosition()
        {
            return GetLevelGrid()
                .GetGridPosition(transform.position);
        }

        private void ApplyGridPosition()
        {
            var gridPosition = GetGridPosition();
            
            if (gridPosition == GridPosition) return;

            var levelGrid = GetLevelGrid();
            levelGrid.RemoveUnitFromGridPosition(this, GridPosition);
            levelGrid.AddUnitToGridPosition(this, gridPosition);
            GridPosition = gridPosition;
        }

        private void Die()
        {
            Destroy(gameObject);
            var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            ragdoll.Setup(rootBone);
        }

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}
