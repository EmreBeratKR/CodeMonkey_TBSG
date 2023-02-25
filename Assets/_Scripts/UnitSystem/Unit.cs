using System.Collections.Generic;
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour
    {
        public GridPosition GridPosition { get; private set; }


        private BaseCommand[] m_Commands;
        private MoveCommand m_MoveCommand;
        private SpinCommand m_SpinCommand;


        private void Awake()
        {
            m_Commands = GetComponents<BaseCommand>();
            m_MoveCommand = GetComponent<MoveCommand>();
            m_SpinCommand = GetComponent<SpinCommand>();
        }

        private void Start()
        {
            GridPosition = GetGridPosition();
            GetLevelGrid().AddUnitToGridPosition(this, GridPosition);
        }


        private void Update()
        {
            ApplyGridPosition();
        }


        public BaseCommand GetDefaultCommand()
        {
            return m_Commands[0];
        }
        
        public IReadOnlyList<BaseCommand> GetAllCommands()
        {
            return m_Commands;
        }

        public MoveCommand GetMoveCommand()
        {
            return m_MoveCommand;
        }

        public SpinCommand GetSpinCommand()
        {
            return m_SpinCommand;
        }
        
        
        private GridPosition GetGridPosition()
        {
            return GetLevelGrid()
                .GetGridPosition(transform.position);
        }

        private void ApplyGridPosition()
        {
            var gridPosition = GetGridPosition();
            
            if (gridPosition == GridPosition) return;

            var levelGrid = GetLevelGrid();
            levelGrid.RemoveUnitFromGridPosition(this, GridPosition);
            levelGrid.AddUnitToGridPosition(this, gridPosition);
            GridPosition = gridPosition;
        }
        

        private static LevelGrid GetLevelGrid()
        {
            return ServiceLocator.Get<LevelGrid>();
        }
    }
}
