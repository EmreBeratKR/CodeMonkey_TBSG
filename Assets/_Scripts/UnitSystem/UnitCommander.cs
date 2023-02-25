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


        public BaseCommand SelectedCommand { get; private set; }
        public Unit SelectedUnit { get; private set; }
    
    
        private void Update()
        {
            if (GameInput.IsMouseOverUI()) return;

            if (TrySelectUnit()) return;
        
            TryExecuteCommand(SelectedCommand);
        }


        public void SetSelectedCommand(BaseCommand command)
        {
            if (command == SelectedCommand) return;
            
            SelectedCommand = command;
            OnSelectedCommandChanged?.Invoke(new SelectedCommandChangedArgs
            {
                command = command
            });
        }
        

        private bool TrySelectUnit()
        {
            if (!Input.GetMouseButtonDown(0)) return false;
        
            var selection = GameInput.GetMouseSelection<Unit>();

            if (!selection) return false;

            if (selection == SelectedUnit) return false;
        
            SelectUnit(selection);

            return true;
        }
    
        private void SelectUnit(Unit unit)
        {
            SelectedUnit = unit;
            OnSelectedUnitChanged?.Invoke(new SelectedUnitChangedArgs
            {
                unit = unit
            });
            
            SetSelectedCommand(unit.GetDefaultCommand());
        }
    
    
        private static bool TryExecuteCommand(BaseCommand command)
        {
            if (!command) return false;
            
            if (!Input.GetMouseButtonDown(0)) return false;

            var mousePosition = GameInput.GetMouseWorldPosition();
        
            if (!mousePosition.HasValue) return false;

            var levelGrid = GetLevelGrid();
            var mouseGridPosition = levelGrid.GetGridPosition(mousePosition.Value);
            var isValidGridPosition = command
                .IsValidGridPosition(mouseGridPosition);

            if (!isValidGridPosition) return false;
            
            ExecuteCommand(command, new CommandArgs
            {
                positionToMove = levelGrid.GetWorldPosition(mouseGridPosition)
            });
            
            return true;
        }

        private static void ExecuteCommand(BaseCommand command, CommandArgs args)
        {
            command.Execute(args);
            OnCommandExecuted?.Invoke(new CommandExecutedArgs
            {
                command = command,
                args = args
            });
        }

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}