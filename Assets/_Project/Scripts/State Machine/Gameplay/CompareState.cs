public class CompareState : State<GameManager>
{
    public CompareState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        GameManager.Instance.Gameplay.ResolveRound();
    }
}