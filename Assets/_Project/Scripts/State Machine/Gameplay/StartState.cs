public class StartState : State<GameManager>
{
    public StartState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        context.Gameplay.GenerateCards();
        context.UIManager.ShowScreen<ScoreTurnCountOverlay>();
        owner.ChangeState<SelectState>();
    }
}