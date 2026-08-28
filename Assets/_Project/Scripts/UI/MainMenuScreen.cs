using UnityEngine;
using UnityEngine.UI;

public class MainMenuScreen : UIScreen
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    protected override void Awake()
    {
        base.Awake();
        playButton.onClick.AddListener(OnPlayClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        GameManager.Instance.StateMachine.TransitionToScene<GameplayState>("GameplayScene");
    }

    private void OnCreditsClicked()
    {
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.UIManager.PushScreen<CreditsMenuScreen>();
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}