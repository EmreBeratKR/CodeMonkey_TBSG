using UnityEngine;

namespace GridSystem
{
    public class Grid
    {
        private const float DefaultCellSize = 1f;
        private const int DefaultDimensionSize = 1;
        private static readonly Vector3 DefaultOrigin = Vector3.zero;


        private GridObject[,,] m_GridObjects;
        private Vector3 m_Origin;
        private float m_CellSize;
        private int m_SizeX;
        private int m_SizeY;
        private int m_SizeZ;


        public Grid(int sizeX, int sizeZ)
        {
            Construct(sizeX, DefaultDimensionSize, sizeZ, DefaultCellSize, DefaultOrigin);
        }
        
        public Grid(int sizeX, int sizeZ, float cellSize)
        {
            Construct(sizeX, DefaultDimensionSize, sizeZ, cellSize, DefaultOrigin);
        }
        
        public Grid(int sizeX, int sizeZ, float cellSize, Vector3 origin)
        {
            Construct(sizeX, DefaultDimensionSize, sizeZ, cellSize, origin);
        }
        
        public Grid(int sizeX, int sizeY, int sizeZ)
        {
            Construct(sizeX, sizeY, sizeZ, DefaultCellSize, DefaultOrigin);
        }
        
        public Grid(int sizeX, int sizeY, int sizeZ, float cellSize)
        {
            Construct(sizeX, sizeY, sizeZ, cellSize, DefaultOrigin);
        }
        
        public Grid(int sizeX, int sizeY, int sizeZ, float cellSize, Vector3 origin)
        {
            Construct(sizeX, sizeY, sizeZ, cellSize, origin);
        }


        public Vector3 GetWorldPosition(int x, int y, int z)
        {
            return m_Origin + new Vector3(x, y, z) * m_CellSize;
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return GetWorldPosition(gridPosition.x, gridPosition.y, gridPosition.z);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            var rawGridPosition = (worldPosition - m_Origin) / m_CellSize;
            var x = Mathf.RoundToInt(rawGridPosition.x);
            var y = Mathf.RoundToInt(rawGridPosition.y);
            var z = Mathf.RoundToInt(rawGridPosition.z);
            return new GridPosition(x, y, z);
        }

        public GridObject GetGridObject(int x, int y, int z)
        {
            return m_GridObjects[x, y, z];
        }

        public GridObject GetGridObject(GridPosition gridPosition)
        {
            return GetGridObject(gridPosition.x, gridPosition.y, gridPosition.z);
        }

        public int GetSizeX()
        {
            return m_SizeX;
        }

        public int GetSizeY()
        {
            return m_SizeY;
        }

        public int GetSizeZ()
        {
            return m_SizeZ;
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            if (gridPosition.x < 0 || gridPosition.x >= m_SizeX) return false;
            
            if (gridPosition.y < 0 || gridPosition.y >= m_SizeY) return false;

            return gridPosition.z >= 0 && gridPosition.z < m_SizeZ;
        }

        public void SpawnGridVisuals(GridVisual visualPrefab, Transform parent = null)
        {
            for (var x = 0; x < m_SizeX; x++)
            {
                for (var y = 0; y < m_SizeY; y++)
                {
                    for (var z = 0; z < m_SizeZ; z++)
                    {
                        var gridPosition = new GridPosition(x, y, z);
                        var newGridObject = new GridObject(this, gridPosition);
                        newGridObject.SpawnVisual(visualPrefab, parent);
                        m_GridObjects[x, y, z] = newGridObject;
                    }
                }
            }
        }

        public void SpawnDebugGridObjects(GridDebugObject prefab, Transform parent = null)
        {
            for (var x = 0; x < m_SizeX; x++)
            {
                for (var y = 0; y < m_SizeY; y++)
                {
                    for (var z = 0; z < m_SizeZ; z++)
                    {
                        var gridDebugObject = Object
                            .Instantiate(prefab, GetWorldPosition(x, y, z), Quaternion.identity, parent);
                        gridDebugObject.SetGridObject(GetGridObject(x, y, z));
                    }
                }
            }
        }


        private void Construct(int sizeX, int sizeY, int sizeZ, float cellSize, Vector3 origin)
        {
            m_SizeX = sizeX;
            m_SizeY = sizeY;
            m_SizeZ = sizeZ;
            m_CellSize = cellSize;
            m_Origin = origin;
            
            m_GridObjects = new GridObject[sizeX, sizeY, sizeZ];
        }
    }
}