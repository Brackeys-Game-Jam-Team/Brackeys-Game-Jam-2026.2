public class EndState : State<GameManager>
{
    public EndState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        var gameplay = GameManager.Instance.Gameplay;
        gameplay.AnnounceWinners();

        foreach (var winner in gameplay.Winners)
            winner.Laugh();

        GameManager.Instance.AudioManager.PlayVoice("Applause");
        GameManager.Instance.UIManager.PushScreen<ResultsScreen>();
        GameManager.Instance.UIManager.HideScreen<ScoreTurnCountOverlay>();
    }
}