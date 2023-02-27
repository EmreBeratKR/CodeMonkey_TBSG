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
    
    private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
    {
        if (args.team == TeamType.Player)
        {
            UpdateVisual();
            return;
        }
        
        HideAll();
    }
    

    private static void UpdateVisual()
    {
        var unitCommander = GetUnitCommander();

        HideAll();
        
        if (unitCommander.IsBusy()) return;
        
        if (!unitCommander.SelectedCommand) return;
        
        var allValidGridPosition = unitCommander
            .SelectedCommand
            .GetAllGridPositionStates();
        
        Show(allValidGridPosition);
    }
    

    private static void Show(IEnumerator<(GridPosition, GridVisual.State)> gridPositions)
    {
        var levelGrid = GetLevelGrid();

        while (gridPositions.MoveNext())
        {
            var gridPosition = gridPositions.Current.Item1;
            var gridVisualState = gridPositions.Current.Item2;
            levelGrid.GetGridObject(gridPosition).SetVisualState(gridVisualState);
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
                        .SetVisualState(GridVisual.State.Clear);
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