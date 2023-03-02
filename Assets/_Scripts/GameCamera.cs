using System;
using Cinemachine;
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using UnityEngine;

[ServiceSceneLoad(ServiceSceneLoadMode.Destroy)]
public class GameCamera : ServiceBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera;
    [SerializeField] private CinemachineImpulseSource rifleFireImpulseSource;
    [SerializeField] private CinemachineImpulseSource explosionImpulseSource;
    [SerializeField] private Transform mainTarget;


    private CinemachineTransposer m_MainCameraTransposer;
    private CinemachineVirtualCamera m_CurrentVirtualCamera;
    private float m_Pitch;
    private float m_Yaw;


    private void Awake()
    {
        m_CurrentVirtualCamera = mainVirtualCamera;
        m_MainCameraTransposer = mainVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        
        ShootCommand.OnAnyShoot += ShootCommand_OnAnyShoot;

        ThrowGrenadeCommand.OnAnyGrenadeExplode += ThrowGrenadeCommand_OnAnyGrenadeExplode;
    }

    private void Start()
    {
        m_Pitch = mainTarget.eulerAngles.x;
    }

    private void OnDestroy()
    {
        ShootCommand.OnAnyShoot -= ShootCommand_OnAnyShoot;
        
        ThrowGrenadeCommand.OnAnyGrenadeExplode -= ThrowGrenadeCommand_OnAnyGrenadeExplode;
    }

    private void Update()
    {
        if (!IsUsingMainVirtualCamera()) return;
        
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    
    private void ShootCommand_OnAnyShoot(ShootCommand.AttackArgs args)
    {
        rifleFireImpulseSource.GenerateImpulse();
    }

    private void ThrowGrenadeCommand_OnAnyGrenadeExplode()
    {
        explosionImpulseSource.GenerateImpulse();
    }
    

    private void HandleMovement()
    {
        var motion = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            motion.z += 1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            motion.z -= 1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            motion.x += 1f;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            motion.x -= 1f;
        }

        const float moveSpeed = 10f;
        motion = motion.normalized * (Time.deltaTime * moveSpeed);
        motion = Quaternion.Euler(Vector3.up * m_Yaw) * motion;
        mainTarget.position += motion;
    }

    private void HandleRotation()
    {
        if (!Input.GetMouseButton(1)) return;

        const float sensitivity = 0.5f;
        var sensitiveMouseDelta = GameInput.MouseDelta * sensitivity;
        const float minPitch = 0f;
        const float maxPitch = 85f;
        m_Pitch = Mathf.Clamp(m_Pitch - sensitiveMouseDelta.y, minPitch, maxPitch);
        m_Yaw += sensitiveMouseDelta.x;

        var eulerAngles = mainTarget.eulerAngles;
        eulerAngles.x = m_Pitch;
        eulerAngles.y = m_Yaw;
        mainTarget.eulerAngles = eulerAngles;
    }

    private void HandleZoom()
    {
        const float minZoom = 1f;
        const float maxZoom = 15f;
        var followOffset = m_MainCameraTransposer.m_FollowOffset;
        followOffset.z = Mathf.Clamp(followOffset.z + Input.mouseScrollDelta.y, -maxZoom, -minZoom);
        m_MainCameraTransposer.m_FollowOffset = followOffset;
    }

    private bool IsUsingMainVirtualCamera()
    {
        return m_CurrentVirtualCamera == mainVirtualCamera;
    }
    
    
    public static void ActivateCamera(CinemachineVirtualCamera virtualCamera)
    {
        SetVirtualCamera(virtualCamera);
        virtualCamera.gameObject.SetActive(true);
    }

    public static void DeactivateCamera(CinemachineVirtualCamera virtualCamera)
    {
        UseMainVirtualCamera();
        virtualCamera.gameObject.SetActive(false);
    }


    private static void UseMainVirtualCamera()
    {
        var instance = GetInstance();
        instance.m_CurrentVirtualCamera = instance.mainVirtualCamera;
    }
    
    private static void SetVirtualCamera(CinemachineVirtualCamera virtualCamera)
    {
        GetInstance().m_CurrentVirtualCamera = virtualCamera;
    }
    
    private static GameCamera GetInstance()
    {
        return ServiceLocator.Get<GameCamera>();
    }
}