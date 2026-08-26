using UnityEngine;

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
        //gameplay.Winners;
    }
}