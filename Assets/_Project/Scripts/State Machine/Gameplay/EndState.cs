public class EndState : State<GameManager>
{
    public EndState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        var gameplay = GameManager.Instance.Gameplay;
        gameplay.AnnounceWinners();
        GameManager.Instance.AudioManager.StopMusic();
        GameManager.Instance.UIManager.PushScreen<ResultsScreen>();
        GameManager.Instance.UIManager.HideScreen<ScoreTurnCountOverlay>();
    }
}