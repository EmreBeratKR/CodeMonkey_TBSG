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
                var position = transform.position;
                var delta = m_CameraTransform.position - position;
                var lookAt = position + delta * (invert ? -1f : 1f); 
                transform.LookAt(lookAt);
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