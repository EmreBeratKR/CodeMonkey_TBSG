using System.Collections.Generic;
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
        HideAll();
        
        if (UnitCommander.IsBusy()) return;

        var selectedCommand = UnitCommander.GetSelectedCommand();
        
        if (!selectedCommand) return;
        
        var allValidGridPosition = selectedCommand.GetAllGridPositionStates();
        
        Show(allValidGridPosition);
    }
    

    private static void Show(IEnumerator<(GridPosition, GridVisual.State)> gridPositions)
    {
        while (gridPositions.MoveNext())
        {
            var gridPosition = gridPositions.Current.Item1;
            var gridVisualState = gridPositions.Current.Item2;
            LevelGrid.GetGridObject(gridPosition).SetVisualState(gridVisualState);
        }
        
        gridPositions.Dispose();
    }
    
    private static void HideAll()
    {
        for (var x = 0; x < LevelGrid.GetSizeX(); x++)
        {
            for (var y = 0; y < LevelGrid.GetSizeY(); y++)
            {
                for (var z = 0; z < LevelGrid.GetSizeZ(); z++)
                {
                    var gridPosition = new GridPosition(x, y, z);
                    LevelGrid
                        .GetGridObject(gridPosition)
                        .SetVisualState(GridVisual.State.Clear);
                }
            }
        }
    }
}