using EmreBeratKR.ServiceLocator;
using UnityEngine;

public class UnitCommander : ServiceBehaviour
{
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