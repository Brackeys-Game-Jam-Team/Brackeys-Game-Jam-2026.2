using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTurnCountOverlay : UIScreen
{
    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI playerScoreText = null;
    [SerializeField] private TextMeshProUGUI turnCountText = null;
    [SerializeField] private RawImage textOverlayImage = null;

    [Header("UI Image Parameters")]
    [Tooltip("Determine overlay image width padding for score if its length is equal to or greater than turn count's length")]
    [SerializeField] private float imageWidthForScorePadding = 10.0f;
    [Tooltip("Determine overlay image width padding for turn count if its length is greater than score's length")]
    [SerializeField] private float imageWidthForTurnCountPadding = 10.0f;

    private Vector2 overlayImageInitialSize = Vector2.zero;
    private int currentLength = 0;

    private Gameplay gameplay => GameManager.Instance.Gameplay;

    protected override void OnShow()
    {
        // Prevent errors by checking if one of the objects is null
        //if (textOverlayImage == null || playerScoreText == null || turnCountText == null || gameplay == null) return;

        playerScoreText.text = "" + gameplay.players[0].Score; // DANIEL: Will need score text for each player, but maybe this one can remain just for the player.
        //turnCountText.text = "Turn " + gameplay.turnCount;

        // Set initial text overlay image size to update width properly whenever score length changes
        //overlayImageInitialSize = textOverlayImage.rectTransform.sizeDelta;

        // Set the current length to be the score's length to string
        //currentLength = gameplay.players[0].Score.ToString().Length;

        AIScoreOverlay aiScoreOverlayObject = FindAnyObjectByType<AIScoreOverlay>();
        if (aiScoreOverlayObject != null) aiScoreOverlayObject.EnableOverlay();
    }

    public void UpdatePlayerScoreText()
    {
        // Prevent errors by checking if one of the objects is null
        //if (textOverlayImage == null || playerScoreText == null || turnCountText == null || gameplay == null) return;

        // Update score text
        playerScoreText.text = "" + gameplay.players[0].Score;

        // Update overlay image width as needed
        DetermineOverlayImageWidth();
    }

    public void UpdateTurnCountText()
    {
        return;

        // Prevent errors by checking if one of the objects is null
        if (textOverlayImage == null || playerScoreText == null || turnCountText == null || gameplay == null) return;

        // Update turn count text
        turnCountText.text = "Turn " + gameplay.turnCount;

        // Update overlay image width as needed
        DetermineOverlayImageWidth();
    }

    // This function is private since overlay image width will be determined after the score and turn count texts are updated
    private void DetermineOverlayImageWidth()
    {
        return;
        // Check if score's length is greater than turn count's length
        if (gameplay.players[0].Score.ToString().Length > gameplay.turnCount.ToString().Length)
        {
            // Update text overlay image size if current length doesn't match the score's length
            if (currentLength != gameplay.players[0].Score.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForScorePadding * (gameplay.players[0].Score.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = gameplay.players[0].Score.ToString().Length;
            }
        }

        // Check if turn count's length is greater than score's length
        else if (gameplay.turnCount.ToString().Length > gameplay.players[0].Score.ToString().Length)
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
            if (currentLength != gameplay.players[0].Score.ToString().Length)
            {
                textOverlayImage.rectTransform.sizeDelta = new Vector2(
                    overlayImageInitialSize.x + (imageWidthForScorePadding * (gameplay.players[0].Score.ToString().Length - 1)),
                    overlayImageInitialSize.y);

                currentLength = gameplay.players[0].Score.ToString().Length;
            }
        }
    }
}
