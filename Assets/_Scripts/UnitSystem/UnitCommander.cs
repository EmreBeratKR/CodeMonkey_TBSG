using System;
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using UnityEngine;

namespace UnitSystem
{
    public class UnitCommander : ServiceBehaviour
    {
        public static event Action<SelectedUnitChangedArgs> OnSelectedUnitChanged;
        public struct SelectedUnitChangedArgs
        {
            public Unit unit;
        }

        public static event Action<SelectedCommandChangedArgs> OnSelectedCommandChanged;
        public struct SelectedCommandChangedArgs
        {
            public BaseCommand command;
        }

        public static event Action<CommandExecutedArgs> OnCommandExecuted;
        public struct CommandExecutedArgs
        {
            public BaseCommand command;
            public CommandArgs args;
        }

        public static event Action<BusyChangedArgs> OnBusyChanged;
        public struct BusyChangedArgs
        {
            public bool isBusy;
        }


        private BaseCommand m_SelectedCommand;
        private Unit m_SelectedUnit;
        private bool m_IsMyTurn;
        private bool m_IsBusy;


        private void Awake()
        {
            TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
        }

        private void OnDestroy()
        {
            TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
        }

        private void Update()
        {
            if (m_IsBusy) return;
            
            if (!m_IsMyTurn) return;

            if (GameInput.IsMouseOverUI()) return;

            if (TrySelectUnit()) return;
        
            TryExecuteCommand(m_SelectedCommand);
        }

        
        private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
        {
            m_IsMyTurn = args.team == TeamType.Player;
        }


        private bool TrySelectUnit()
        {
            if (!Input.GetMouseButtonDown(0)) return false;
        
            var selection = GameInput.GetMouseSelection<Unit>();

            if (!selection) return false;

            if (selection == m_SelectedUnit) return false;

            if (!selection.IsInsideTeam(TeamType.Player)) return false;
        
            SelectUnit(selection);

            return true;
        }
    
        private void SelectUnit(Unit unit)
        {
            m_SelectedUnit = unit;
            OnSelectedUnitChanged?.Invoke(new SelectedUnitChangedArgs
            {
                unit = unit
            });
            
            SetSelectedCommand(unit.GetDefaultCommand());
        }

        private void SetBusy(bool value)
        {
            m_IsBusy = value;
            OnBusyChanged?.Invoke(new BusyChangedArgs
            {
                isBusy = value
            });
        }
        
        private bool TryExecuteCommand(BaseCommand command)
        {
            if (!command) return false;
            
            if (!Input.GetMouseButtonDown(0)) return false;

            var mousePosition = GameInput.GetMouseWorldPosition();
        
            if (!mousePosition.HasValue) return false;
            
            var mouseGridPosition = LevelGrid.GetGridPosition(mousePosition.Value);
            var isValidGridPosition = command
                .IsValidGridPosition(mouseGridPosition);

            if (!isValidGridPosition) return false;
            
            if (!command.Unit.TryUseCommandPoint(command)) return false;
            
            ExecuteCommand(command, new CommandArgs
            {
                gridPositionToMove = mouseGridPosition,
                unitToShoot = LevelGrid.GetUnitAtGridPosition(mouseGridPosition)
            });
            
            return true;
        }

        private void ExecuteCommand(BaseCommand command, CommandArgs args)
        {
            SetBusy(true);
            
            command.Execute(args, () => SetBusy(false));
            OnCommandExecuted?.Invoke(new CommandExecutedArgs
            {
                command = command,
                args = args
            });
        }
        
        
        public static void SetSelectedCommand(BaseCommand command)
        {
            var instance = GetInstance();
            
            if (command == instance.m_SelectedCommand) return;
            
            instance.m_SelectedCommand = command;
            OnSelectedCommandChanged?.Invoke(new SelectedCommandChangedArgs
            {
                command = command
            });
        }

        public static Unit GetSelectedUnit()
        {
            return GetInstance().m_SelectedUnit;
        }

        public static BaseCommand GetSelectedCommand()
        {
            return GetInstance().m_SelectedCommand;
        }
        
        public static bool IsBusy()
        {
            return GetInstance().m_IsBusy;
        }


        private static UnitCommander GetInstance()
        {
            return ServiceLocator.Get<UnitCommander>();
        }
    }
}