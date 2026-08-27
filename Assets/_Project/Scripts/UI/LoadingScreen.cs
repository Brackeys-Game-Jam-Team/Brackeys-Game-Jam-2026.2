using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : UIScreen
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMPro.TextMeshProUGUI loadingText;

    protected override void OnShow()
    {
        if (progressBar != null)
            progressBar.value = 0f;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        // Could read progress from GameManager.Instance.LoadProgress
        // For now just animate
        if (progressBar != null)
            progressBar.value = Mathf.MoveTowards(progressBar.value, 1f, Time.deltaTime * 2f);
    }
}