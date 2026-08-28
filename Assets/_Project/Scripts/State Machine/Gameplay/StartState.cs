public class StartState : State<GameManager>
{
    public StartState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        GameManager.Instance.Gameplay.StartGame();
        GameManager.Instance.UIManager.ShowScreen<ScoreTurnCountOverlay>();
        GameManager.Instance.AudioManager.PlayMusic("Gameplay");
        owner.ChangeState<SelectState>();
    }
}