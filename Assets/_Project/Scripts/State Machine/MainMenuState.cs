using UnityEngine;

public class MainMenuState : State<GameManager>
{
    public MainMenuState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        Debug.Log("Open the main menu UI");
        GameManager.Instance.InputManager.EnableUI();
        GameManager.Instance.LoadScene("MainMenu");


        owner.ChangeState<GameplayState>();
    }
}