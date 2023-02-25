using System;
using EmreBeratKR.ServiceLocator;
using GridSystem;
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


        public Unit SelectedUnit { get; private set; }
    
    
        private void Update()
        {
            if (TrySelectUnit()) return;
        
            TryCommandUnitToMove(SelectedUnit);
        }


        private bool TrySelectUnit()
        {
            if (!Input.GetMouseButtonDown(0)) return false;
        
            var selection = GameInput.GetMouseSelection<Unit>();

            if (!selection) return false;
        
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
        }
    
    
        private static bool TryCommandUnitToMove(Unit unit)
        {
            if (!unit) return false;
            
            if (!Input.GetMouseButtonDown(0)) return false;

            var mousePosition = GameInput.GetMouseWorldPosition();
        
            if (!mousePosition.HasValue) return false;

            var levelGrid = GetLevelGrid();
            var mouseGridPosition = levelGrid.GetGridPosition(mousePosition.Value);
            var isValidGridPosition = unit
                .GetMoveCommand()
                .IsValidGridPosition(mouseGridPosition);

            if (!isValidGridPosition) return false;

            CommandUnitToMove(unit, levelGrid.GetWorldPosition(mouseGridPosition));

            return true;
        }
    
        private static void CommandUnitToMove(Unit unit, Vector3 position)
        {
            if (!unit) return;
        
            unit
                .GetMoveCommand()
                .Move(position);
        }

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}