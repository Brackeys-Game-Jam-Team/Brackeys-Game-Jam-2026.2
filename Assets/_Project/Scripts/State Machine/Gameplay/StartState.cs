public class StartState : State<GameManager>
{
    public StartState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        GameManager.Instance.Gameplay.GenerateCards();
        owner.ChangeState<SelectState>();
    }
}