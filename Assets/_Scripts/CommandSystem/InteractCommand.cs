using System;
using System.Collections.Generic;
using GridSystem;

namespace CommandSystem
{
    public class InteractCommand : BaseCommand
    {
        private const float InteractRange = 1f;


        private void OnInteractionComplete()
        {
            CompleteCommand();
        }
        
        
        public override void Execute(CommandArgs args, Action onCompleted)
        {
            args.interactable.Interact(Unit, OnInteractionComplete);
            StartCommand(onCompleted);
        }

        public override IEnumerator<(GridPosition, GridVisual.State, CommandStatus)> GetAllGridPositionStates()
        {
            var allGridPositionsWithinRange = GetAllGridPositionWithinRange(InteractRange);

            while (allGridPositionsWithinRange.MoveNext())
            {
                var gridPosition = allGridPositionsWithinRange.Current;

                if (gridPosition == Unit.GridPosition)
                {
                    yield return (gridPosition, GridVisual.State.Clear, CommandStatus.BadSelection);
                    continue;
                }
                
                if (!HasEnoughCommandPoint())
                {
                    yield return (gridPosition, GridVisual.State.Red, CommandStatus.NotEnoughCommandPoint);
                    continue;
                }
                
                var interactable = LevelGrid.GetInteractableAtGridPosition(gridPosition);

                if (interactable == null)
                {
                    yield return (gridPosition, GridVisual.State.DarkBlue, CommandStatus.TargetNotFound);
                    continue;
                }

                if (!interactable.IsValid())
                {
                    yield return (gridPosition, GridVisual.State.Orange, CommandStatus.Blocked);
                    continue;
                }

                yield return (gridPosition, GridVisual.State.Blue, CommandStatus.Ok);
            }
            
            allGridPositionsWithinRange.Dispose();
        }

        public override string GetName()
        {
            const string commandName = "Interact";
            return commandName;
        }
    }
}