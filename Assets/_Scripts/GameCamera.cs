using Cinemachine;
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using General;
using UnityEngine;

[ServiceSceneLoad(ServiceSceneLoadMode.Destroy)]
public class GameCamera : ServiceBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera;
    [SerializeField] private CinemachineImpulseSource rifleFireImpulseSource;
    [SerializeField] private CinemachineImpulseSource explosionImpulseSource;
    [SerializeField] private Transform mainTarget;
    [SerializeField] private FloatRange xPositionRange;
    [SerializeField] private FloatRange zPositionRange;


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

        MeleeCommand.OnAnyHit += MeleeCommand_OnAnyHit;
    }

    private void Start()
    {
        m_Pitch = mainTarget.eulerAngles.x;
    }

    private void OnDestroy()
    {
        ShootCommand.OnAnyShoot -= ShootCommand_OnAnyShoot;
        
        ThrowGrenadeCommand.OnAnyGrenadeExplode -= ThrowGrenadeCommand_OnAnyGrenadeExplode;
        
        MeleeCommand.OnAnyHit -= MeleeCommand_OnAnyHit;
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
    
    private void MeleeCommand_OnAnyHit()
    {
        explosionImpulseSource.GenerateImpulse();
    }
    

    private void HandleMovement()
    {
        var input = GameInput.GetCameraMovement();
        var motion = new Vector3(input.x, 0f, input.y);
        
        const float moveSpeed = 10f;
        motion = motion.normalized * (Time.deltaTime * moveSpeed);
        motion = Quaternion.Euler(Vector3.up * m_Yaw) * motion;
        var position = mainTarget.position + motion;
        position.x = xPositionRange.Clamp(position.x);
        position.z = zPositionRange.Clamp(position.z);
        mainTarget.position = position;
    }

    private void HandleRotation()
    {
        const float sensitivity = 1.5f;
        var sensitiveRotation = GameInput.GetCameraRotation() * sensitivity;
        const float minPitch = 0f;
        const float maxPitch = 85f;
        m_Pitch = Mathf.Clamp(m_Pitch - sensitiveRotation.y, minPitch, maxPitch);
        m_Yaw += sensitiveRotation.x;

        var eulerAngles = mainTarget.eulerAngles;
        eulerAngles.x = m_Pitch;
        eulerAngles.y = m_Yaw;
        mainTarget.eulerAngles = eulerAngles;
    }

    private void HandleZoom()
    {
        const float sensitivity = 0.03f;
        const float minZoom = 1f;
        const float maxZoom = 15f;
        var followOffset = m_MainCameraTransposer.m_FollowOffset;
        var sensitiveZoom = GameInput.GetCameraZoom() * sensitivity;
        followOffset.z = Mathf.Clamp(followOffset.z + sensitiveZoom, -maxZoom, -minZoom);
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