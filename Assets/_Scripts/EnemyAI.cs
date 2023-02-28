using System.Collections.Generic;
using AI;
using CommandSystem;
using EmreBeratKR.ServiceLocator;
using UI;

public class EnemyAI : BehaviourTree, IService
{
    private readonly List<BaseCommand> m_CommandsBuffer = new();
    private TeamType m_TeamType;
    private bool m_IsBusy;
    
    
    protected override void Awake()
    {
        base.Awake();
        
        TurnManager.OnTurnChanged += TurnManager_OnTurnChanged;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnChanged -= TurnManager_OnTurnChanged;
    }


    private void TurnManager_OnTurnChanged(TurnManager.TurnChangedArgs args)
    {
        m_TeamType = args.team;
    }

    
    protected override Node SetupTree()
    {
        var rootNode = new SequenceNode();

        var waitForTurnNode = new WaitUntilNode(() => m_TeamType == TeamType.Enemy);
        var repeatNode = new RepeatUntilNode(() =>
        {
            return !m_IsBusy && !CanPerformMoreCommand();
        });
        var performCommandNode = new SequenceNode();
        var thinkNode = new WaitForSecondsNode(0.25f);
        var executeCommandNode = new ActionNode(() =>
        {
            return TryExecuteCommand() ? NodeState.Succeed : NodeState.Failure;
        });
        var waitCommandToCompleteNode = new WaitUntilNode(() => !m_IsBusy);
        var giveTurnNode = new ActionNode(() =>
        {
            GetTurnManager().NextTurn();
            return NodeState.Succeed;
        });
        
        rootNode.AddNode(waitForTurnNode);
        rootNode.AddNode(repeatNode);
        rootNode.AddNode(giveTurnNode);
        
        repeatNode.AddNode(performCommandNode);
        
        performCommandNode.AddNode(thinkNode);
        performCommandNode.AddNode(executeCommandNode);
        performCommandNode.AddNode(waitCommandToCompleteNode);

        return rootNode;
    }


    private bool CanPerformMoreCommand()
    {
        var units = UnitManager.GetUnitsWithTeamType(TeamType.Enemy);

        foreach (var unit in units)
        {
            if (unit.CommandPoint > 0) return true;
        }

        return false;
    }
    
    private bool TryExecuteCommand()
    {
        var (bestCommand, bestCommandArgs) = GetBestCommandAndArgsPair();

        if (!bestCommand) return false;
        
        m_IsBusy = true;
        
        bestCommand.Execute(bestCommandArgs, () =>
        {
            m_IsBusy = false;
        });

        bestCommand.Unit.TryUseCommandPoint(bestCommand);

        return true;
    }

    private (BaseCommand, CommandArgs) GetBestCommandAndArgsPair()
    {
        var units = UnitManager.GetUnitsWithTeamType(TeamType.Enemy);
        
        m_CommandsBuffer.Clear();

        foreach (var unit in units)
        {
            var commands = unit.GetAllCommands();

            foreach (var command in commands)
            {
                if (!unit.HasEnoughCommandPoint(command)) continue;
                
                m_CommandsBuffer.Add(command);
            }
        }

        BaseCommand bestCommand = null;
        var bestCommandArgs = new CommandArgs();
        var bestBenefitValue = float.MinValue;

        foreach (var command in m_CommandsBuffer)
        {
            var subBestCommandArgs = GetBestCommandArgs(command);
            var benefitValue = command.GetBenefitValue(subBestCommandArgs);
            
            if (benefitValue <= bestBenefitValue) continue;

            bestCommand = command;
            bestCommandArgs = subBestCommandArgs;
            bestBenefitValue = benefitValue;
        }

        return (bestCommand, bestCommandArgs);
    }

    private CommandArgs GetBestCommandArgs(BaseCommand command)
    {
        var levelGrid = GetLevelGrid();
        
        var bestArgs = new CommandArgs();
        var bestBenefitValue = float.MinValue;
        
        var allValidGridPositions = command.GetAllValidGridPositions();

        while (allValidGridPositions.MoveNext())
        {
            var gridPosition = allValidGridPositions.Current;
            var args = new CommandArgs
            {
                gridPositionToMove = gridPosition,
                unitToShoot = levelGrid.GetUnitAtGridPosition(gridPosition)
            };
            var benefitValue = command.GetBenefitValue(args);
            
            if (benefitValue <= bestBenefitValue) continue;

            bestArgs = args;
            bestBenefitValue = benefitValue;
        }
        
        allValidGridPositions.Dispose();

        return bestArgs;
    }


    private static TurnManager GetTurnManager()
    {
        return ServiceLocator.Get<TurnManager>();
    }

    private static LevelGrid GetLevelGrid()
    {
        return ServiceLocator.Get<LevelGrid>();
    }
}