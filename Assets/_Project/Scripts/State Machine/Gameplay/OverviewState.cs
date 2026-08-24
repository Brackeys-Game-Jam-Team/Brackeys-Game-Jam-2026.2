using UnityEngine;

public class OverviewState : State<GameManager>
{
    public OverviewState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        Debug.Log("Open the overview UI");
    }
}