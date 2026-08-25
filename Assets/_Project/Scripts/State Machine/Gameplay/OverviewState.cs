using UnityEngine;
using UnityEngine.InputSystem;

public class OverviewState : State<GameManager>
{
    private InputSystem_Actions.PlayerActions PlayerActions => GameManager.Instance.InputManager.InputActions.Player;

    public OverviewState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        Debug.Log("Open the overview UI");
        PlayerActions.Interact.performed += OnButtonPressed;
    }

    public override void Exit()
    {
        Debug.Log("Hide the overview UI");
        PlayerActions.Interact.performed -= OnButtonPressed;
    }

    private void OnButtonPressed(InputAction.CallbackContext context)
    {
        owner.ChangeState<StartState>();
    }
}