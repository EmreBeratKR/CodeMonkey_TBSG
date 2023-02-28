using TMPro;
using UnityEngine;

namespace PathfindingSystem
{
    public class GridPathfindingDebugObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text gCostField;
        [SerializeField] private TMP_Text hCostField;
        [SerializeField] private TMP_Text fCostField;
        [SerializeField] private SpriteRenderer isWalkableSprite;


        private PathNode m_PathNode;


        private void Update()
        {
            if (m_PathNode.GCost == int.MaxValue)
            {
                gCostField.text = "?";
                hCostField.text = "?";
                fCostField.text = "?";
                return;
            }
            
            gCostField.text = m_PathNode.GCost.ToString();
            hCostField.text = m_PathNode.HCost.ToString();
            fCostField.text = m_PathNode.FCost.ToString();

            isWalkableSprite.color = m_PathNode.IsWalkable()
                ? Color.green
                : Color.red;
        }


        public void SetPathNode(PathNode pathNode)
        {
            m_PathNode = pathNode;
        }
    }
}