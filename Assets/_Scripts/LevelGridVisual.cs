using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;

public class LevelGridVisual : MonoBehaviour
{
    private void Update()
    {
        var unitCommander = GetUnitCommander();

        HideAll();
        
        if (!unitCommander.SelectedUnit) return;
        
        var allValidGridPosition = unitCommander
            .SelectedUnit
            .GetMoveCommand()
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