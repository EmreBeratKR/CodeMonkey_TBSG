using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;

public class LevelGridVisual : MonoBehaviour
{
    private void Awake()
    {
        UnitCommander.OnSelectedCommandChanged += UnitCommander_OnSelectedCommandChanged;
        UnitCommander.OnBusyChanged += UnitCommander_OnBusyChanged;
        
        TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
    }

    private void OnDestroy()
    {
        UnitCommander.OnSelectedCommandChanged -= UnitCommander_OnSelectedCommandChanged;
        UnitCommander.OnBusyChanged -= UnitCommander_OnBusyChanged;
        
        TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
    }

    
    private void UnitCommander_OnSelectedCommandChanged(UnitCommander.SelectedCommandChangedArgs args)
    {
        UpdateVisual();
    }

    private void UnitCommander_OnBusyChanged(UnitCommander.BusyChangedArgs args)
    {
        UpdateVisual();
    }
    
    private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs obj)
    {
        UpdateVisual();
    }
    

    private void UpdateVisual()
    {
        var unitCommander = GetUnitCommander();

        HideAll();
        
        if (unitCommander.IsBusy()) return;
        
        if (!unitCommander.SelectedCommand) return;
        
        var allValidGridPosition = unitCommander
            .SelectedCommand
            .GetAllValidGridPositions();
        
        Show(allValidGridPosition);
    }
    

    private static void Show(IEnumerator<GridPosition> gridPositions)
    {
        var levelGrid = GetLevelGrid();

        while (gridPositions.MoveNext())
        {
            var gridPosition = gridPositions.Current;
            levelGrid.GetGridObject(gridPosition).SetActiveVisual(true);
        }
        
        gridPositions.Dispose();
    }
    
    private static void HideAll()
    {
        var levelGrid = GetLevelGrid();

        for (var x = 0; x < levelGrid.GetSizeX(); x++)
        {
            for (var y = 0; y < levelGrid.GetSizeY(); y++)
            {
                for (var z = 0; z < levelGrid.GetSizeZ(); z++)
                {
                    var gridPosition = new GridPosition(x, y, z);
                    levelGrid
                        .GetGridObject(gridPosition)
                        .SetActiveVisual(false);
                }
            }
        }
    }


    private static UnitCommander GetUnitCommander()
    {
        return ServiceLocator.Get<UnitCommander>();
    }
    
    private static LevelGrid GetLevelGrid()
    {
        return ServiceLocator.Get<LevelGrid>();
    }
}