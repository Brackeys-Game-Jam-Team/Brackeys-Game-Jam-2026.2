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
        GameManager.Instance.InputManager.EnablePlayer();
        GameManager.Instance.LoadScene("GameplayScene");
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}