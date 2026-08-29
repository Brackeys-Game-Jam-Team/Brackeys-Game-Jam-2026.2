using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AIScoreOverlay : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private List<TextMeshProUGUI> aiScoreTexts = null;
    private RawImage textOverlayImage = null;

    [Header("UI Image Parameters")]
    [Tooltip("Determine overlay image width padding for score if its length is equal to or greater than turn count's length")]
    [SerializeField] private float imageWidthForScorePadding = 10.0f;

    private Vector2 overlayImageInitialSize = Vector2.zero;
    private int currentLength = 0;

    private Gameplay gameplay => GameManager.Instance.Gameplay;

    private void Awake()
    {
        textOverlayImage = GetComponent<RawImage>();
    }

    public void EnableOverlay()
    {
        gameObject.SetActive(true);

        // Prevent errors by checking if one of the objects is null
        //if (textOverlayImage == null || gameplay == null) return;

        int greatestScoreLength = 0;

        for (int i = 0; i < aiScoreTexts.Count; i++)
        {
            aiScoreTexts[i].text = $"" + gameplay.players[i + 1].Score;

            // Make sure the greatest score length is set to the player that has the greatest score length
            if (greatestScoreLength < gameplay.players[i + 1].Score.ToString().Length)
            {
                greatestScoreLength = gameplay.players[i + 1].Score.ToString().Length;
            }
        }

        // Set initial text overlay image size to update width properly whenever score length changes
        //overlayImageInitialSize = textOverlayImage.rectTransform.sizeDelta;

        // Set the current length to be the score's length to string
        //currentLength = greatestScoreLength;
    }

    public void DisableOverlay()
    {
        gameObject.SetActive(false);
    }

    public void UpdateAIScoreTexts()
    {
        // Prevent errors by checking if one of the objects is null
        //if (textOverlayImage == null || gameplay == null) return;

        int greatestScoreLength = 0;

        for (int i = 0; i < aiScoreTexts.Count; i++)
        {
            aiScoreTexts[i].text = $"" + gameplay.players[i + 1].Score;

            // Make sure the greatest score length is set to the player that has the greatest score length
            if (greatestScoreLength < gameplay.players[i + 1].Score.ToString().Length)
            {
                greatestScoreLength = gameplay.players[i + 1].Score.ToString().Length;
            }
        }

        // Update overlay image width as needed by getting the greatest score length value
        DetermineOverlayImageWidth(greatestScoreLength);
    }

    // This function is private since overlay image width will be determined after the score and turn count texts are updated
    private void DetermineOverlayImageWidth(int greatestScoreLength)
    {
        return;
        // Update text overlay image size if current length doesn't match the score's length
        if (currentLength != greatestScoreLength)
        {
            textOverlayImage.rectTransform.sizeDelta = new Vector2(
                overlayImageInitialSize.x + (imageWidthForScorePadding * (greatestScoreLength - 1)),
                overlayImageInitialSize.y);

            currentLength = greatestScoreLength;
        }
    }
}
