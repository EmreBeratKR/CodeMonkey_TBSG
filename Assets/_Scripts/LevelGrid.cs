using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;

public class LevelGrid : ServiceBehaviour
{
    public const int SizeX = 20;
    public const int SizeZ = 20;
    
    
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


    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return m_Grid.GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return m_Grid.GetWorldPosition(gridPosition);
    }

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return m_Grid.GetGridObject(gridPosition);
    }

    public int GetSizeX()
    {
        return m_Grid.GetSizeX();
    }

    public int GetSizeY()
    {
        return m_Grid.GetSizeY();
    }

    public int GetSizeZ()
    {
        return m_Grid.GetSizeZ();
    }
    
    public void AddUnitToGridPosition(Unit unit, GridPosition gridPosition)
    {
        var gridObject = m_Grid.GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public void RemoveUnitFromGridPosition(Unit unit, GridPosition gridPosition)
    {
        var gridObject = m_Grid.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public bool HasAnyUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = m_Grid.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        var gridObject = m_Grid.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return m_Grid.IsValidGridPosition(gridPosition);
    }
}