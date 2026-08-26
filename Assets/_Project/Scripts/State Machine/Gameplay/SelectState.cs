public class SelectState : State<GameManager>
{
    public SelectState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        GameManager.Instance.Gameplay.turnCount++;
        GameManager.Instance.CanSelectCard = true;
        // open score turn count overlay UI
    }

    public override void Exit()
    {
        GameManager.Instance.CanSelectCard = false;
    }
}