using EmreBeratKR.ServiceLocator;
using UnityEngine;

public class GameInput : ServiceBehaviour
{
    [SerializeField] private LayerMask mouseRaycastLayerMask;
    [SerializeField] private LayerMask mouseSelectionLayerMask;
    
    
    private Camera m_Camera;
    
    
    public static Vector3? GetMouseWorldPosition()
    {
        var instance = GetInstance();
        var ray = instance.GetMousePositionRay();

        var isHit = Physics
            .Raycast(ray, out var hitInfo, float.MaxValue, instance.mouseRaycastLayerMask);

        return isHit ? hitInfo.point : null;
    }

    public static Collider GetMouseSelection()
    {
        var instance = GetInstance();
        var ray = instance.GetMousePositionRay();

        var isHit = Physics
            .Raycast(ray, out var hitInfo, float.MaxValue, instance.mouseSelectionLayerMask);

        return isHit ? hitInfo.collider : null;
    }

    public static T GetMouseSelection<T>()
        where T : Component
    {
        var selection = GetMouseSelection();

        if (!selection) return null;

        return selection.TryGetComponent(out T castedSelection)
            ? castedSelection
            : null;
    }
    
    
    private Camera GetCamera()
    {
        if (!m_Camera)
        {
            m_Camera = Camera.main;
        }

        return m_Camera;
    }

    private Ray GetMousePositionRay()
    {
        return GetCamera()
            .ScreenPointToRay(Input.mousePosition);
    }
    
    
    private static GameInput GetInstance()
    {
        return ServiceLocator.Get<GameInput>();
    }
}