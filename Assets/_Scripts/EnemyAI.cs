using AI;
using EmreBeratKR.ServiceLocator;

public class EnemyAI : BehaviourTree, IService
{
    private TeamType m_TeamType;
    
    
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
        var thinkNode = new WaitForSecondsNode(1f);
        var giveTurnNode = new ActionNode(() =>
        {
            GetTurnManager().NextTurn();
            return NodeState.Succeed;
        });
        
        rootNode.AddNode(waitForTurnNode);
        rootNode.AddNode(thinkNode);
        rootNode.AddNode(giveTurnNode);

        return rootNode;
    }


    private static TurnManager GetTurnManager()
    {
        return ServiceLocator.Get<TurnManager>();
    }
}