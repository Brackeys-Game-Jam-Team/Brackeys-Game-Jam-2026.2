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
        GameManager.Instance.AudioManager.PlaySFX("ButtonClick2");
        GameManager.Instance.StateMachine.TransitionToScene<GameplayState>("GameplayScene");
    }

    private void OnCreditsClicked()
    {
        GameManager.Instance.AudioManager.PlaySFX("ButtonClick");
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.UIManager.PushScreen<CreditsMenuScreen>();
    }

    private void OnQuitClicked()
    {
        GameManager.Instance.AudioManager.PlaySFX("ButtonClick");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}