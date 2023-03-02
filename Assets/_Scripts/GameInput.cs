using EmreBeratKR.ServiceLocator;
using UnityEngine;
using UnityEngine.EventSystems;

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


    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public static bool IsLeftMouseButtonDown()
    {
        return Input.GetMouseButtonDown(0);
    }
    
    public static bool IsRightMouseButton()
    {
        return Input.GetMouseButton(1);
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

    public static Vector2 GetCameraMovement()
    {
        var motion = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            motion.y += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            motion.y -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            motion.x += 1f;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            motion.x -= 1f;
        }

        return motion;
    }

    public static float GetCameraZoom()
    {
        return Input.mouseScrollDelta.y;
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