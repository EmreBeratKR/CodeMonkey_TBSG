using EmreBeratKR.ServiceLocator;
using UnityEngine;

namespace Debug
{
    public class DebugWorldMouse : MonoBehaviour
    {
        [SerializeField] private GameObject visual;
        
        
        private void Update()
        {
            var mouseWorldPosition = GameInput.GetMouseWorldPosition();

            visual.SetActive(mouseWorldPosition.HasValue);
            transform.position = mouseWorldPosition.GetValueOrDefault();
        }
    }
}