public class CheckConditionState : State<GameManager>
{
    public CheckConditionState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        if (GameManager.Instance.Gameplay.CheckGameCondition())
            owner.ChangeState<EndState>();

        else
            owner.ChangeState<SelectState>();
    }
}