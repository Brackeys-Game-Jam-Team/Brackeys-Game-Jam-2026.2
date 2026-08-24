public class GameplayState : StateMachine
{
    public GameplayState(StateMachine owner) : base(owner)
    {
        AddState(new OverviewState(this));
        AddState(new StartState(this));
        AddState(new SelectState(this));
        AddState(new CompareState(this));
        AddState(new CheckConditionState(this));
        AddState(new EndState(this));
        SetDefaultState<OverviewState>();
    }

    public override void Enter()
    {
        base.Enter();
        GameManager.Instance.InputManager.EnablePlayer();
    }

    public override void Exit()
    {
        base.Exit();
    }
}