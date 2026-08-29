public class SelectState : State<GameManager>
{
    public SelectState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        GameManager.Instance.Gameplay.TurnCount++;
        GameManager.Instance.CanSelectCard = true;
        var scoreUI = GameManager.Instance.UIManager.GetScreen<ScoreTurnCountOverlay>();
        scoreUI.UpdateTurnCountText();
    }

    public override void Exit()
    {
        GameManager.Instance.CanSelectCard = false;
    }
}