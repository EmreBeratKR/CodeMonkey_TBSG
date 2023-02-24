using System;
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


        private Unit m_SelectedUnit;
    
    
        private void Update()
        {
            if (TrySelectUnit()) return;
        
            TryCommandUnitToMove(m_SelectedUnit);
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
            m_SelectedUnit = unit;
            OnSelectedUnitChanged?.Invoke(new SelectedUnitChangedArgs
            {
                unit = unit
            });
        }
    
    
        private static bool TryCommandUnitToMove(Unit unit)
        {
            if (!Input.GetMouseButtonDown(0)) return false;

            var mousePosition = GameInput.GetMouseWorldPosition();
        
            if (!mousePosition.HasValue) return false;
        
            CommandUnitToMove(unit, mousePosition.Value);

            return true;
        }
    
        private static void CommandUnitToMove(Unit unit, Vector3 position)
        {
            if (!unit) return;
        
            unit.Move(position);
        }
    }
}