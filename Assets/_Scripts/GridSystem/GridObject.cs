using System;
using System.Collections.Generic;
using System.Text;
using UnitSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridObject
    {
        private GridPosition m_GridPosition;
        private Grid m_Grid;
        private readonly List<Unit> m_Units = new();


        public GridObject(Grid grid, GridPosition gridPosition)
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