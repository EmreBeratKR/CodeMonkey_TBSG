using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

public class LevelGrid : ServiceBehaviour
{
    [SerializeField] private GridVisual gridVisualPrefab;
    [SerializeField] private GridDebugObject gridDebugObjectPrefab;
    [SerializeField] private bool spawnDebugGridObjects = true;
        
        
    private readonly Grid m_Grid = new(10, 10);


    private void Start()
    {
        m_Grid.SpawnGridVisuals(gridVisualPrefab, transform);
        
        if (spawnDebugGridObjects)
        {
            m_Grid.SpawnDebugGridObjects(gridDebugObjectPrefab, transform);
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

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        if (!m_Grid.IsValidGridPosition(gridPosition)) return false;
        
        var gridObject = m_Grid.GetGridObject(gridPosition);

        return !gridObject.HasAnyUnit();
    }
}