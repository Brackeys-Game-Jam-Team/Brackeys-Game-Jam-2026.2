using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuScreen : UIScreen
{
    [SerializeField] private Button backButton;

    protected override void Awake()
    {
        base.Awake();
        backButton.onClick.AddListener(OnBackToMainMenuClicked);
    }

    protected override void OnShow()
    {
        GameManager.Instance.AudioManager.PlayVoice("Applause");
    }

    private void OnBackToMainMenuClicked()
    {
        GameManager.Instance.AudioManager.PlaySFX("ButtonClick");
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.UIManager.PushScreen<MainMenuScreen>();
    }
}
