using System.Collections.Generic;
using EmreBeratKR.ServiceLocator;
using UnityEngine;
using GridSystem;

namespace PathfindingSystem
{
    public class Pathfinding : ServiceBehaviour
    {
        private const int CostPerStraightMove = 10;
        private const int CostPerDiagonalMove = 14;
        
        private static readonly Grid<PathNode> Grid = new(LevelGrid.SizeX, LevelGrid.SizeZ);
        private static readonly HashSet<PathNode> NodesToVisit = new(); 
        private static readonly HashSet<PathNode> VisitedNodes = new();
        private static readonly List<PathNode> NeighbourNodes = new();
        private static readonly List<Vector3> EmptyPath = new();


        [SerializeField] private GridPathfindingDebugObject debugObjectPrefab;
        [SerializeField] private bool spawnDebugGridObjects = true;


        private void Start()
        {
            Grid.SpawnGridObjects((grid, gridPosition) =>
            {
                var pathNode = new PathNode();
                pathNode.GridPosition = gridPosition;
                pathNode.GridObject = ServiceLocator.Get<LevelGrid>().GetGridObject(gridPosition);
                return pathNode;
            });

            if (spawnDebugGridObjects)
            {
                Grid.SpawnDebugGridObjects((gridPosition) =>
                {
                    var gridDebugObject = Instantiate(debugObjectPrefab, Grid.GetWorldPosition(gridPosition), Quaternion.identity, transform);
                    gridDebugObject.SetPathNode(Grid.GetGridObject(gridPosition));
                });
            }
        }


        public static IReadOnlyList<Vector3> GetPath(GridPosition startGridPosition, GridPosition endGridPosition)
        {
            CleanUp();

            var startPathNode = Grid.GetGridObject(startGridPosition);
            var endPathNode = Grid.GetGridObject(endGridPosition);

            startPathNode.GCost = 0;
            startPathNode.HCost = CalculateHCost(startPathNode, endPathNode);
            startPathNode.CalculateFCost();
            NodesToVisit.Add(startPathNode);

            while (NodesToVisit.Count > 0)
            {
                var pathNode = GetPathNodeWithLowestFCost(NodesToVisit);

                if (pathNode == endPathNode)
                {
                    return CalculatePathFromEndPathNode(pathNode);
                }

                NodesToVisit.Remove(pathNode);
                VisitedNodes.Add(pathNode);

                var neighbourPathNodes = GetAllNeighbourPathNodes(pathNode);

                foreach (var neighbourPathNode in neighbourPathNodes)
                {
                    if (!neighbourPathNode.IsWalkable()) continue;
                    
                    if (VisitedNodes.Contains(neighbourPathNode)) continue;
                    
                    var gCost = pathNode.GCost + CalculateHCost(pathNode, neighbourPathNode);
                    
                    if (gCost > neighbourPathNode.GCost) continue;

                    neighbourPathNode.GCost = gCost;
                    neighbourPathNode.HCost = CalculateHCost(neighbourPathNode, endPathNode);
                    neighbourPathNode.CalculateFCost();
                    
                    if (NodesToVisit.Contains(neighbourPathNode)) continue;

                    neighbourPathNode.PreviousPathNode = pathNode;
                    NodesToVisit.Add(neighbourPathNode);
                }
            }

            return EmptyPath;
        }


        private static IReadOnlyList<Vector3> CalculatePathFromEndPathNode(PathNode endPathNode)
        {
            var path = new List<Vector3>();

            var currentPathNode = endPathNode;
            while (true)
            {
                if (currentPathNode == null) break;
                
                path.Add(Grid.GetWorldPosition(currentPathNode.GridPosition));
                currentPathNode = currentPathNode.PreviousPathNode;
            }
            
            path.Reverse();

            return path;
        }

        private static IReadOnlyList<PathNode> GetAllNeighbourPathNodes(PathNode pathNode)
        {
            NeighbourNodes.Clear();

            var upperGridPosition = pathNode.GridPosition + new GridPosition(0, 1);
            if (Grid.IsValidGridPosition(upperGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(upperGridPosition));
            }
            
            var upperRightGridPosition = pathNode.GridPosition + new GridPosition(1, 1);
            if (Grid.IsValidGridPosition(upperRightGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(upperRightGridPosition));
            }
            
            var rightGridPosition = pathNode.GridPosition + new GridPosition(1, 0);
            if (Grid.IsValidGridPosition(rightGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(rightGridPosition));
            }
            
            var lowerRightGridPosition = pathNode.GridPosition + new GridPosition(1, -1);
            if (Grid.IsValidGridPosition(lowerRightGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(lowerRightGridPosition));
            }
            
            var lowerGridPosition = pathNode.GridPosition + new GridPosition(0, -1);
            if (Grid.IsValidGridPosition(lowerGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(lowerGridPosition));
            }
            
            var lowerLeftGridPosition = pathNode.GridPosition + new GridPosition(-1, -1);
            if (Grid.IsValidGridPosition(lowerLeftGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(lowerLeftGridPosition));
            }
            
            var leftGridPosition = pathNode.GridPosition + new GridPosition(-1, 0);
            if (Grid.IsValidGridPosition(leftGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(leftGridPosition));
            }
            
            var upperLeftGridPosition = pathNode.GridPosition + new GridPosition(-1, 1);
            if (Grid.IsValidGridPosition(upperLeftGridPosition))
            {
                NeighbourNodes.Add(Grid.GetGridObject(upperLeftGridPosition));
            }

            return NeighbourNodes;
        }

        private static PathNode GetPathNodeWithLowestFCost(IEnumerable<PathNode> pathNodes)
        {
            PathNode pathNodeWithLowestFCost = null;
            var lowestFCost = int.MaxValue;

            foreach (var pathNode in pathNodes)
            {
                if (pathNode.FCost > lowestFCost) continue;

                pathNodeWithLowestFCost = pathNode;
                lowestFCost = pathNode.FCost;
            }

            return pathNodeWithLowestFCost;
        }
        
        private static int CalculateHCost(PathNode fromPathNode, PathNode toPathNode)
        {
            var from = fromPathNode.GridPosition;
            var to = toPathNode.GridPosition;
            
            var distanceX = Mathf.Abs(from.x - to.x);
            var distanceZ = Mathf.Abs(from.z - to.z);
            var remainder = Mathf.Abs(distanceX - distanceZ);
            
            var straightCost = remainder * CostPerStraightMove;
            var diagonalCost = Mathf.Min(distanceX, distanceZ) * CostPerDiagonalMove;
            
            return straightCost + diagonalCost;
        }
        
        private static void CleanUp()
        {
            NodesToVisit.Clear();
            VisitedNodes.Clear();

            for (var x = 0; x < Grid.GetSizeX(); x++)
            {
                for (var y = 0; y < Grid.GetSizeY(); y++)
                {
                    for (var z = 0; z < Grid.GetSizeZ(); z++)
                    {
                        var gridPosition = new GridPosition(x, y, z);
                        var pathNode = Grid.GetGridObject(gridPosition);

                        pathNode.GCost = int.MaxValue;
                        pathNode.HCost = 0;
                        pathNode.CalculateFCost();
                        pathNode.PreviousPathNode = null;
                    }
                }
            }
        }
    }
}