public class MainMenuState : State<GameManager>
{
    public MainMenuState(StateMachine owner) : base(owner)
    {
    }

    public override void Enter()
    {
        context.InputManager.EnableUI();
        context.UIManager.PushScreen<MainMenuScreen>();
        context.LoadScene("MainMenu");
        context.AudioManager.PlayMusic("TitleMusic");
    }
    public override void Exit()
    {
        context.UIManager.ClearStack();
    }
}