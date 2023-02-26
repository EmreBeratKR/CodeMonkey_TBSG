using EmreBeratKR.ServiceLocator;
using UnitSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridVisual : MonoBehaviour
    {
        [SerializeField] private Renderer visual;
        
        
        public void SetActive(bool value)
        {
            if (value)
            {
                SetState(GetState());
                return;
            }
            
            SetState(GridVisualState.Unreachable);
        }


        private GridVisualState GetState()
        {
            var unitCommander = GetUnitCommander();
            var selectedCommand = unitCommander.SelectedCommand;
            
            return selectedCommand.Unit.HasEnoughCommandPoint(selectedCommand) switch
            {
                false => GridVisualState.NotEnoughCommandPoint,
                _ => GridVisualState.Available
            };
        }
        
        private void SetState(GridVisualState state)
        {
            visual.material.color = state switch
            {
                GridVisualState.Available => Color.white,
                GridVisualState.NotEnoughCommandPoint => Color.red,
                GridVisualState.Unreachable => Color.black
            };
        }


        private static UnitCommander GetUnitCommander()
        {
            return ServiceLocator.Get<UnitCommander>();
        }


        private enum GridVisualState
        {
            Available,
            NotEnoughCommandPoint,
            Unreachable
        }
    }
}