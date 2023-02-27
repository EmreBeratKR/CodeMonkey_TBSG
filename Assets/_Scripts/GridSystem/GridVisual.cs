using EmreBeratKR.ServiceLocator;
using UnitSystem;
using UnityEngine;

namespace GridSystem
{
    public class GridVisual : MonoBehaviour
    {
        [SerializeField] private Renderer visual;
        

        public void SetState(State state)
        {
            visual.material.color = state switch
            {
                State.Clear => Color.black,
                State.White => Color.white,
                State.Orange => new Color(1f, 0.5f, 0f),
                State.Blue => new Color(0f, 0.8f, 1f),
                State.DarkBlue => new Color(0f, 0.4f, 0.5f),
                State.Green => Color.green,
            };
        }


        private static UnitCommander GetUnitCommander()
        {
            return ServiceLocator.Get<UnitCommander>();
        }


        public enum State
        {
            Clear,
            White,
            Orange,
            Blue,
            DarkBlue,
            Green,
        }
    }
}