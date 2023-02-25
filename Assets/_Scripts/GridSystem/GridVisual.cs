using UnityEngine;

namespace GridSystem
{
    public class GridVisual : MonoBehaviour
    {
        [SerializeField] private GameObject visual;
        
        
        public void SetActive(bool value)
        {
            visual.SetActive(value);
        }
    }
}