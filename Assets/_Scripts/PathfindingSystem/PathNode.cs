using GridSystem;

namespace PathfindingSystem
{
    public class PathNode
    {
        public int GCost { get; set; }
        public int HCost { get; set; }
        public int FCost { get; private set; }
        public PathNode PreviousPathNode { get; set; }
        public GridPosition GridPosition { get; set; }


        public void CalculateFCost()
        {
            FCost = GCost + HCost;
        }
    }
}