public class SelectState : State<GameManager>
{
    public SelectState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        context.Gameplay.turnCount++;
        context.CanSelectCard = true;
        var scoreUI = context.UIManager.GetScreen<ScoreTurnCountOverlay>();
        scoreUI.UpdateTurnCountText();
    }

    public override void Exit()
    {
        GameManager.Instance.CanSelectCard = false;
    }
}