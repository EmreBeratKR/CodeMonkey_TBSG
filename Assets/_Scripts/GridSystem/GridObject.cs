using System.Collections.Generic;
using System.Text;
using UnitSystem;
using UnityEngine;
using WeaponSystem;

namespace GridSystem
{
    public class GridObject : IBulletTarget
    {
        private const int StaticObstacleBitmask = 1 << 8;
        private static readonly Collider[] ColliderBuffer = new Collider[10];
        
        
        private GridPosition m_GridPosition;
        private GridVisual m_Visual;
        private Grid<GridObject> m_Grid;
        private readonly List<Unit> m_Units = new();
        private readonly List<IObstacle> m_Obstacles = new();


        public GridObject(Grid<GridObject> grid, GridPosition gridPosition)
        {
            m_Grid = grid;
            m_GridPosition = gridPosition;
            CheckObstacles();
        }


        public Vector3 GetHitPosition()
        {
            return GetWorldPosition();
        }
        
        public Vector3 GetWorldPosition()
        {
            return m_Grid.GetWorldPosition(m_GridPosition);
        }
        
        public void AddUnit(Unit unit)
        {
            m_Units.Add(unit);
            m_Obstacles.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            m_Units.Remove(unit);
            m_Obstacles.Remove(unit);
        }

        public bool HasAnyUnit()
        {
            return m_Units.Count > 0;
        }

        public Unit GetUnit()
        {
            return m_Units.Count > 0 
                ? m_Units[0]
                : null;
        }

        public void SetVisualState(GridVisual.State state)
        {
            m_Visual.SetState(state);
        }

        public void SpawnVisual(GridVisual visualPrefab, Transform parent = null)
        {
            m_Visual = Object.Instantiate(visualPrefab, parent);
            m_Visual.transform.position = GetWorldPosition();
        }

        public bool HasAnyObstacle()
        {
            return m_Obstacles.Count > 0;
        }

        public bool IsWalkable()
        {
            return !HasAnyObstacle();
        }
        
        
        public override string ToString()
        {
            var stringBuilder = new StringBuilder(m_GridPosition.ToString());

            foreach (var obstacle in m_Obstacles)
            {
                stringBuilder.Append($"\n{((MonoBehaviour) obstacle).name}");
            }
            
            return stringBuilder.ToString();
        }


        private void CheckObstacles()
        {
            var center = GetWorldPosition();
            const float radius = 0.1f;

            var count = Physics
                .OverlapSphereNonAlloc(center, radius, ColliderBuffer, StaticObstacleBitmask);

            for (var i = 0; i < count; i++)
            {
                if (!ColliderBuffer[i].TryGetComponent(out IObstacle obstacle)) return;
                
                m_Obstacles.Add(obstacle);
            }
        }
    }
}