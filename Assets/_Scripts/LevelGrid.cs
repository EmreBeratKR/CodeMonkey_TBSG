using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;

public class LevelGrid : ServiceBehaviour
{
    public const int SizeX = 19;
    public const int SizeZ = 32;
    
    
    [SerializeField] private GridVisual gridVisualPrefab;
    [SerializeField] private GridDebugObject gridDebugObjectPrefab;
    [SerializeField] private bool spawnDebugGridObjects = true;
        
        
    private readonly Grid<GridObject> m_Grid = new(SizeX, SizeZ);


    private void Start()
    {
        m_Grid.SpawnGridObjects((grid, gridPosition) =>
        {
            var newGridObject = new GridObject(grid, gridPosition);
            newGridObject.SpawnVisual(gridVisualPrefab, transform);
            return newGridObject;
        });
        
        if (spawnDebugGridObjects)
        {
            m_Grid.SpawnDebugGridObjects((gridPosition) =>
            {
                var gridDebugObject = Instantiate(gridDebugObjectPrefab, GetWorldPosition(gridPosition), Quaternion.identity, transform);
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            });
        }
    }


    public static GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return GetInstance().m_Grid.GetGridPosition(worldPosition);
    }

    public static Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return GetInstance().m_Grid.GetWorldPosition(gridPosition);
    }

    public static GridObject GetGridObject(GridPosition gridPosition)
    {
        return GetInstance().m_Grid.GetGridObject(gridPosition);
    }

    public static int GetSizeX()
    {
        return GetInstance().m_Grid.GetSizeX();
    }

    public static int GetSizeY()
    {
        return GetInstance().m_Grid.GetSizeY();
    }

    public static int GetSizeZ()
    {
        return GetInstance().m_Grid.GetSizeZ();
    }
    
    public static void AddUnitToGridPosition(Unit unit, GridPosition gridPosition)
    {
        var gridObject = GetInstance().m_Grid.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public static void RemoveUnitFromGridPosition(Unit unit, GridPosition gridPosition)
    {
        var gridObject = GetInstance().m_Grid.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public static bool HasAnyUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = GetInstance().m_Grid.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public static Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = GetInstance().m_Grid.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public static bool IsValidGridPosition(GridPosition gridPosition)
    {
        return GetInstance().m_Grid.IsValidGridPosition(gridPosition);
    }


    private static LevelGrid GetInstance()
    {
        return ServiceLocator.Get<LevelGrid>();
    }
}