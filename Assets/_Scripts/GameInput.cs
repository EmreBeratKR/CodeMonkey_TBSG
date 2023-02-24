using EmreBeratKR.ServiceLocator;
using UnityEngine;

public class GameInput : ServiceBehaviour
{
    [SerializeField] private LayerMask mouseRaycastLayerMask;
    
    
    private Camera m_Camera;
    
    
    public static Vector3? GetMouseWorldPosition()
    {
        var instance = ServiceLocator.Get<GameInput>();

        var ray = instance.GetCamera()
            .ScreenPointToRay(Input.mousePosition);

        var isHit = Physics
            .Raycast(ray, out var hitInfo, float.MaxValue, instance.mouseRaycastLayerMask);

        return isHit ? hitInfo.point : null;
    }


    private Camera GetCamera()
    {
        if (!m_Camera)
        {
            m_Camera = Camera.main;
        }

        return m_Camera;
    }
}