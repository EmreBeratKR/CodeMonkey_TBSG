using GridSystem;
using UnityEngine;

public class Obstacle : MonoBehaviour, IObstacle
{
    private GridObject m_GridObject;
    
    
    public void SetGridObject(GridObject gridObject)
    {
        m_GridObject = gridObject;
    }
}