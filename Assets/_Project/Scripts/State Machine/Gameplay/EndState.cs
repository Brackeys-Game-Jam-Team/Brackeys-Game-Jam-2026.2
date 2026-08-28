public class EndState : State<GameManager>
{
    public EndState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        var gameplay = GameManager.Instance.Gameplay;
        gameplay.AnnounceWinners();
        // open End UI here
        GameManager.Instance.UIManager.PushScreen<ResultsScreen>();
        GameManager.Instance.UIManager.HideScreen<ScoreTurnCountOverlay>();
    }
}