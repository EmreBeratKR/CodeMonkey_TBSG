namespace AI
{
    public class SequenceNode : Node
    {
        public override NodeState Tick()
        {
            var currentChildNodeState = TickCurrentChildNode();

            switch (currentChildNodeState)
            {
                case NodeState.Running:
                    return NodeState.Running;
                
                case NodeState.Succeed:
                    return MoveNextChildNode() 
                        ? NodeState.Running 
                        : NodeState.Succeed;
                
                default:
                    return NodeState.Failure;
            }
        }
    }
}