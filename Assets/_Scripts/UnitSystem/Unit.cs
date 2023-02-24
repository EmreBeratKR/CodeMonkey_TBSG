using CommandSystem;
using EmreBeratKR.ServiceLocator;
using GridSystem;
using UnityEngine;

namespace UnitSystem
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private MoveCommand moveCommand;
        
        
        public GridPosition GridPosition { get; private set; }


        private void Start()
        {
            GridPosition = GetGridPosition();
            GetLevelGrid().AddUnitToGridPosition(this, GridPosition);
        }


        private void Update()
        {
            ApplyGridPosition();
        }


        public MoveCommand GetMoveCommand()
        {
            return moveCommand;
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
