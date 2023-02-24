using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnitSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

public class LevelGrid : ServiceBehaviour
{
    [SerializeField] private GridDebugObject gridDebugObjectPrefab;
    [SerializeField] private bool spawnDebugGridObjects = true;
        
        
    private readonly Grid m_Grid = new(10, 10);


    private void Start()
    {
        if (spawnDebugGridObjects)
        {
            m_Grid.SpawnDebugGridObjects(gridDebugObjectPrefab, transform);
        }
    }


    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return m_Grid.GetGridPosition(worldPosition);
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
}