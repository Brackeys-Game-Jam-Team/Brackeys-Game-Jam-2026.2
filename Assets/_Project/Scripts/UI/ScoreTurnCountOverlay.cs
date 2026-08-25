using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTurnCountOverlay : MonoBehaviour
{
    [Header("Integer Values")]

    [Tooltip("These are integer values to test the overlay image size updating properly")]
    [SerializeField] private int score = 0;
    [Tooltip("These are integer values to test the overlay image size updating properly")]
    [SerializeField] private int turnCount = 0;

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI turnCountText = null;
    [SerializeField] private RawImage textOverlayImage = null;

    [Header("UI Image Parameters")]
    [Tooltip("Determine overlay image width padding for score if its length is equal to or greater than turn count's length")]
    [SerializeField] private float imageWidthForScorePadding = 10.0f;
    [Tooltip("Determine overlay image width padding for turn count if its length is greater than score's length")]
    [SerializeField] private float imageWidthForTurnCountPadding = 10.0f;

    private Vector2 overlayImageInitialSize = Vector2.zero;
    private int currentLength = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Score: " + score;
        turnCountText.text = "Turn Count: " + turnCount;

        // Set initial text overlay image size to update width properly whenever score length changes
        overlayImageInitialSize = textOverlayImage.rectTransform.sizeDelta;

        // Set the current length to be the score's length to string
        currentLength = score.ToString().Length;
    }

    // Update is called once per frame
    void Update()
    {
        // Update score and turn count texts
        scoreText.text = "Score: " + score;
        turnCountText.text = "Turn Count: " + turnCount;

        // Check if score's length is greater than turn count's length
        if (score.ToString().Length > turnCount.ToString().Length)
        {
            // Update text overlay image size if current length doesn't match the score's length
            if (currentLength != score.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForScorePadding * (score.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = score.ToString().Length;
            }
        }

        // Check if turn count's length is greater than score's length
        else if (turnCount.ToString().Length > score.ToString().Length)
        {
            // Update text overlay image size if current length doesn't match the turn count's length
            if (currentLength != turnCount.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForTurnCountPadding * (turnCount.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = turnCount.ToString().Length;
            }
        }

        // If the score's length is equal to turn count's length
        else
        {
            // Update text overlay image size if current length doesn't match the score's length as default
            if (currentLength != score.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForScorePadding * (score.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = score.ToString().Length;
            }
        }

        // Clamp score to maximum possible value, perhaps
        if (score > 1000000000) score = 1000000000;
    }
}
