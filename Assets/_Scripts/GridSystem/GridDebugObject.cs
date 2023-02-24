using TMPro;
using UnityEngine;

namespace GridSystem
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text debugField;
        
        
        private GridObject m_GridObject;


        private void Update()
        {
            debugField.text = m_GridObject.ToString();
        }


        public void SetGridObject(GridObject gridObject)
        {
            m_GridObject = gridObject;
        }
    }
}