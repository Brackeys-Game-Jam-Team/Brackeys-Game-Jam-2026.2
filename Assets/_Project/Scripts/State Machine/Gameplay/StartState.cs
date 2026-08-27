public class StartState : State<GameManager>
{
    public StartState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        GameManager.Instance.Gameplay.GenerateCards();
        GameManager.Instance.UIManager.ShowScreen<ScoreTurnCountOverlay>();
        owner.ChangeState<SelectState>();
    }
}