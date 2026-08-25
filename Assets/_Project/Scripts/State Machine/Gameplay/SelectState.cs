public class SelectState : State<GameManager>
{
    public SelectState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        base.Enter();
        GameManager.Instance.Gameplay.turnCount++;
        // open score turn count overlay UI
        // enable card selection
    }

    public override void Exit()
    {
        base.Exit();
        // disable card selection
    }
}