public class SelectState : State<GameManager>
{
    public SelectState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // enable card selection
    }

    public override void Exit()
    {
        base.Exit();
        // disable card selection
    }
}