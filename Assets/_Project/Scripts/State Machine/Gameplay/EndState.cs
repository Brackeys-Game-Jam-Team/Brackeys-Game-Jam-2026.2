public class EndState : State<GameManager>
{
    public EndState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        var gameplay = context.Gameplay;
        gameplay.AnnounceWinners();
        // open End UI here
        //gameplay.Winners;
    }
}