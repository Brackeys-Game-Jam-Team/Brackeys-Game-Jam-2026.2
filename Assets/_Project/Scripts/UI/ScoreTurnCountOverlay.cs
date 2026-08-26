using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTurnCountOverlay : MonoBehaviour
{
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

    // DANIEL: Gameplay reference (keeps a list of players' scores and the turn count)
    private Gameplay gameplay => GameManager.Instance.Gameplay;

    // TODO: Find player object to set score text
    private Player player = null;


    void Start()
    {
        // Prevent errors by checking if one of the objects is null
        if (textOverlayImage == null || scoreText == null || turnCountText == null || gameplay == null || player == null) return;

        scoreText.text = "Score: " + player.Score; // DANIEL: Will need score text for each player, but maybe this one can remain just for the player.
        turnCountText.text = "Turn Count: " + gameplay.turnCount;

        // Set initial text overlay image size to update width properly whenever score length changes
        overlayImageInitialSize = textOverlayImage.rectTransform.sizeDelta;

        // Set the current length to be the score's length to string
        currentLength = player.Score.ToString().Length;
    }

    // DANIEL: We can eventually replace update with a function that will be called from the Gameplay script, or from any of the State scripts, but I can deal with that part
    void Update()
    {

    }

    public void UpdateScoreText()
    {
        // Prevent errors by checking if one of the objects is null
        if (textOverlayImage == null || scoreText == null || turnCountText == null || gameplay == null || player == null) return;

        // Update score text
        scoreText.text = "Score: " + player.Score;

        // Update overlay image width as needed
        DetermineOverlayImageWidth();
    }

    public void UpdateTurnCountText()
    {
        // Prevent errors by checking if one of the objects is null
        if (textOverlayImage == null || scoreText == null || turnCountText == null || gameplay == null || player == null) return;

        // Update turn count text
        turnCountText.text = "Turn Count: " + gameplay.turnCount;

        // Update overlay image width as needed
        DetermineOverlayImageWidth();
    }

    // This function is private since overlay image width will be determined after the score and turn count texts are updated
    private void DetermineOverlayImageWidth()
    {
        // Check if score's length is greater than turn count's length
        if (player.Score.ToString().Length > gameplay.turnCount.ToString().Length)
        {
            // Update text overlay image size if current length doesn't match the score's length
            if (currentLength != player.Score.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForScorePadding * (player.Score.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = player.Score.ToString().Length;
            }
        }

        // Check if turn count's length is greater than score's length
        else if (gameplay.turnCount.ToString().Length > player.Score.ToString().Length)
        {
            // Update text overlay image size if current length doesn't match the turn count's length
            if (currentLength != gameplay.turnCount.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForTurnCountPadding * (gameplay.turnCount.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = gameplay.turnCount.ToString().Length;
            }
        }

        // If the score's length is equal to turn count's length
        else
        {
            // Update text overlay image size if current length doesn't match the score's length as default
            if (currentLength != player.Score.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForScorePadding * (player.Score.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = player.Score.ToString().Length;
            }
        }
    }
}
