public class CompareState : State<GameManager>
{
    public CompareState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        context.Gameplay.ResolveRound();
    }
}