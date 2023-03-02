using GridSystem;
using InteractionSystem;
using UnitSystem;

namespace CommandSystem
{
    public struct CommandArgs
    {
        public GridPosition gridPosition;
        public GridObject gridObject;
        public Unit unit;
        public IInteractable interactable;
    }
}