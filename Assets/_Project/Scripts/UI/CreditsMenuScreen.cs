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

    private void OnBackToMainMenuClicked()
    {
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.UIManager.PushScreen<MainMenuScreen>();
    }
}
