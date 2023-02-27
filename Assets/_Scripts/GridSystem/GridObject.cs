using System.Collections.Generic;
using System.Text;
using UnitSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridObject
    {
        private GridPosition m_GridPosition;
        private GridVisual m_Visual;
        private Grid<GridObject> m_Grid;
        private readonly List<Unit> m_Units = new();


        public GridObject(Grid<GridObject> grid, GridPosition gridPosition)
        {
            m_Grid = grid;
            m_GridPosition = gridPosition;
        }


        public void AddUnit(Unit unit)
        {
            m_Units.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            m_Units.Remove(unit);
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
            m_Visual.transform.position = m_Grid.GetWorldPosition(m_GridPosition);
        }
        
        
        public override string ToString()
        {
            var stringBuilder = new StringBuilder(m_GridPosition.ToString());

            foreach (var unit in m_Units)
            {
                stringBuilder.Append($"\n{unit.name}");
            }
            
            return stringBuilder.ToString();
        }
    }
}