using Cinemachine;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera mainVirtualCamera;
    [SerializeField] private Transform mainTarget;


    private CinemachineTransposer m_MainCameraTransposer;
    private float m_Pitch;
    private float m_Yaw;


    private void Awake()
    {
        m_MainCameraTransposer = mainVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Start()
    {
        m_Pitch = mainTarget.eulerAngles.x;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
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

        const float moveSpeed = 5f;
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
}