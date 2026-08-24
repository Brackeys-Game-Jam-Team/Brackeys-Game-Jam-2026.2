using UnityEngine;

public class StartState : State<GameManager>
{
    public StartState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        var i = context.Gameplay;
        i.GenerateCards();

        Debug.Log("Open the overview UI");
    }
}