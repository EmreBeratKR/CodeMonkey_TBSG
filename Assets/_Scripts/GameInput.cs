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


    private InputActions m_Actions;
    private Camera m_Camera;


    private void Awake()
    {
        m_Actions = new InputActions();
        m_Actions.Enable();
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            OnLeftMouseButtonDown?.Invoke();
        }
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

    public static Vector2 GetCameraRotation()
    {
        return GetInstance()
            .m_Actions
            .Player
            .CameraRotation
            .ReadValue<Vector2>();
    }

    public static float GetCameraZoom()
    {
        return GetInstance()
            .m_Actions
            .Player
            .CameraZoom
            .ReadValue<float>();
    }


    private static Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
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