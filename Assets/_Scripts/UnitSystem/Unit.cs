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
        

        public static event Action<UnitUsedCommandPointArgs> OnAnyUnitUsedCommandPoint;
        public struct UnitUsedCommandPointArgs
        {
            public Unit unit;
        }

        public static event Action<Unit> OnAnyUnitSpawned; 
        public static event Action<Unit> OnAnyUnitDead;

        
        public event Action OnUnitUsedCommandPoint;
        
        
        public GridPosition GridPosition { get; private set; }
        public int CommandPoint { get; private set; } = MaxCommandPoint;


        private BaseCommand[] m_Commands;
        private Vector3 m_LastImpactOffset;


        private void Awake()
        {
            m_Commands = GetComponents<BaseCommand>();

            health.OnDead += OnDead;
            
            ShootCommand.OnAnyShoot += OnAnyUnitShoot;
            
            TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
        }

        private void Start()
        {
            Spawn();
        }

        private void OnDestroy()
        {
            health.OnDead -= OnDead;

            ShootCommand.OnAnyShoot -= OnAnyUnitShoot;
            
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

        private void OnAnyUnitShoot(ShootCommand.ShootArgs args)
        {
            if (args.shotUnit == this)
            {
                m_LastImpactOffset = args.impactOffset;
            }
        }
        
        private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
        {
            if (args.team == teamType)
            {
                RestoreCommandPoints();
            }
        }


        public Vector3 GetPosition()
        {
            return transform.position;
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

        public bool TryGetCommand<T>(out T command)
            where T : BaseCommand
        {
            command = GetCommand<T>();
            return command;
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
            OnAnyUnitUsedCommandPoint?.Invoke(new UnitUsedCommandPointArgs
            {
                unit = this
            });
            
            OnUnitUsedCommandPoint?.Invoke();
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

        private void Spawn()
        {
            RestoreCommandPoints();
            GridPosition = GetGridPosition();
            GetLevelGrid().AddUnitToGridPosition(this, GridPosition);
            
            OnAnyUnitSpawned?.Invoke(this);
        }

        private void Die()
        {
            Destroy(gameObject);
            var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
            ragdoll.Setup(rootBone, m_LastImpactOffset);
            
            OnAnyUnitDead?.Invoke(this);
        }

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}
