using EmreBeratKR.ServiceLocator;
using UnityEngine;

public class GameInput : ServiceBehaviour
{
    [SerializeField] private LayerMask mouseRaycastLayerMask;
    [SerializeField] private LayerMask mouseSelectionLayerMask;


    public static Vector2 MouseDelta => GetMouseDelta();
    
    
    private Camera m_Camera;
    private Vector2 m_MouseScreenPositionCurrentFrame;
    private Vector2 m_MouseScreenPositionLastFrame;
    private bool m_IsFirstFrame = true;


    private void Update()
    {
        HandleDoubleBuffering();
    }


    public static Vector2 GetMouseScreenPosition()
    {
        var mousePosition = Input.mousePosition;
        return new Vector2(mousePosition.x, mousePosition.y);
    }
    
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


    private static Vector2 GetMouseDelta()
    {
        var instance = GetInstance();
        return instance.m_MouseScreenPositionCurrentFrame - instance.m_MouseScreenPositionLastFrame;
    }

    private void HandleDoubleBuffering()
    {
        if (m_IsFirstFrame)
        {
            m_IsFirstFrame = false;
            m_MouseScreenPositionLastFrame = GetMouseScreenPosition();
            m_MouseScreenPositionCurrentFrame = m_MouseScreenPositionLastFrame;
            return;
        }

        m_MouseScreenPositionLastFrame = m_MouseScreenPositionCurrentFrame;
        m_MouseScreenPositionCurrentFrame = GetMouseScreenPosition();
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