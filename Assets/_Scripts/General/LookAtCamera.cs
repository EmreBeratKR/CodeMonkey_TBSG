using UnityEngine;

namespace General
{
    public class LookAtCamera : MonoBehaviour
    {
        [SerializeField] private Mode mode;
        [SerializeField] private bool invert;


        private Transform m_CameraTransform;


        private void Awake()
        {
            m_CameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (mode == Mode.LookAt)
            {
                var up = invert ? Vector3.down : Vector3.up;
                transform.LookAt(m_CameraTransform, up);
            }

            if (mode == Mode.LookForward)
            {
                var sign = invert ? -1f : 1f;
                transform.forward = sign * m_CameraTransform.forward;
            }
        }
    
    
        private enum Mode
        {
            LookAt,
            LookForward
        }
    }
}