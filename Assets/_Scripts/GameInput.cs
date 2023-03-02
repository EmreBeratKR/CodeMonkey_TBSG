using System;
using EmreBeratKR.ServiceLocator;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameInput : ServiceBehaviour
{
    [SerializeField] private LayerMask mouseRaycastLayerMask;
    [SerializeField] private LayerMask mouseSelectionLayerMask;


    public static event Action OnLeftMouseButtonDown;


    public static Vector2 MousePosition => GetMouseScreenPosition();
    public static Vector2 MouseDelta => GetMouseDelta();
    public static Vector2 MouseScroll => GetMouseScroll();


    private InputActions m_Actions;
    private Camera m_Camera;


    private void Awake()
    {
        m_Actions = new InputActions();
        m_Actions.Enable();
        
        m_Actions.Player.Enable();
        
        m_Actions.Player.CameraMovement.Enable();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnLeftMouseButtonDown?.Invoke();
        }
    }


    public static bool IsRightMouseButton()
    {
        return Mouse.current.rightButton.isPressed;
    }
    
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
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
        return GetInstance()
            .m_Actions
            .Player
            .CameraMovement
            .ReadValue<Vector2>();
    }


    private static Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }
    
    private static Vector2 GetMouseDelta()
    {
        return Mouse.current.delta.ReadValue();
    }

    private static Vector2 GetMouseScroll()
    {
        return Mouse.current.scroll.ReadValue();
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
            .ScreenPointToRay(GetMouseScreenPosition());
    }
    
    
    private static GameInput GetInstance()
    {
        return ServiceLocator.Get<GameInput>();
    }
}