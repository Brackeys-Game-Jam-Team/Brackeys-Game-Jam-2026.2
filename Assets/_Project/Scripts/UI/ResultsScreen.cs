using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsScreen : UIScreen
{
    [Header("UI Score Text")]
    [SerializeField] private List<TextMeshProUGUI> playerScoreTexts;

    [Header("UI Score Text Colors")]
    [SerializeField] private Color firstPlaceColor = Color.white;
    [SerializeField] private Color secondPlaceColor = Color.white;
    [SerializeField] private Color thirdPlaceColor = Color.white;
    [SerializeField] private Color fourthPlaceColor = Color.white;

    [Header("UI Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;

    private Gameplay gameplay => GameManager.Instance.Gameplay;

    protected override void Awake()
    {
        base.Awake();
        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    protected override void OnShow()
    {
        // Prevent errors by checking if one of the objects is null
        if (gameplay == null || playerScoreTexts.Count <= 0) return;

        int winnersPassed = 0;

        List<bool> winnersUpdated = new List<bool> { false, false, false, false };

        for (int i = 0; i < gameplay.Winners.Count; i++)
        {
            // Set the player score texts depending on the player's and all the AI's scores
            if (gameplay.Winners[i].Score == gameplay.players[0].Score && !winnersUpdated[0])
            {
                playerScoreTexts[i].text = "Player Score: " + gameplay.Winners[i].Score;
                winnersUpdated[0] = true;
            }

            else if (gameplay.Winners[i].Score == gameplay.players[1].Score && !winnersUpdated[1])
            {
                playerScoreTexts[i].text = $"CPU{1} Score: " + gameplay.Winners[i].Score;
                winnersUpdated[1] = true;
            }

            else if (gameplay.Winners[i].Score == gameplay.players[2].Score && !winnersUpdated[2])
            {
                playerScoreTexts[i].text = $"CPU{2} Score: " + gameplay.Winners[i].Score;
                winnersUpdated[2] = true;
            }

            else if (gameplay.Winners[i].Score == gameplay.players[3].Score && !winnersUpdated[3])
            {
                playerScoreTexts[i].text = $"CPU{3} Score: " + gameplay.Winners[i].Score;
                winnersUpdated[3] = true;
            }

            playerScoreTexts[i].color = firstPlaceColor;

            winnersPassed++;
        }

        List<int> scores = new List<int>();

        // Add players scores to the scores list
        for (int i = 0; i < gameplay.players.Count; i++)
        {
            scores.Add(gameplay.players[i].Score);
        }

        // Sort from the highest score to lowest score
        scores.Sort((p1, p2) => p2.CompareTo(p1));

        // This will prevent the text printing duplicating players whenever they have the same score
        List<bool> scoreUpdated = new List<bool>{ false, false, false, false };

        // If there is only one winner, update the text colors depending on score values
        if (winnersPassed == 1)
        {
            // If the second score is greater than the third score and third score greater than the fourth score, set second, third and fourth place text colors
            if (scores[1] > scores[2] && scores[2] > scores[3])
            {
                playerScoreTexts[1].color = secondPlaceColor;
                playerScoreTexts[2].color = thirdPlaceColor;
                playerScoreTexts[3].color = fourthPlaceColor;
            }

            // If the second score is greater than the third score and third score is equal to the fourth score
            else if (scores[1] > scores[2] && scores[2] == scores[3])
            {
                // Set second player score text to second place color and the other player score texts to third place color
                playerScoreTexts[1].color = secondPlaceColor;
                playerScoreTexts[2].color = thirdPlaceColor;
                playerScoreTexts[3].color = thirdPlaceColor;
            }

            // If the second score is equal to the third score and third score is greater than the fourth score
            else if (scores[1] == scores[2] && scores[2] > scores[3])
            {
                // Set second and third player score texts to second place color and the fourth player score text color to third place color
                playerScoreTexts[1].color = secondPlaceColor;
                playerScoreTexts[2].color = secondPlaceColor;
                playerScoreTexts[3].color = thirdPlaceColor;
            }

            // If the second score is equal to the third score and third score is equal to the fourth score
            else if (scores[1] == scores[2] && scores[2] == scores[3])
            {
                // Set the second, third and fourth place score texts to second place color
                playerScoreTexts[1].color = secondPlaceColor;
                playerScoreTexts[2].color = secondPlaceColor;
                playerScoreTexts[3].color = secondPlaceColor;
            }
        }

        // If there are 2 winners, update the text colors depending on score values
        else if (winnersPassed == 2)
        {
            // If the third score is greater than the fourth score, set them to second and third place colors, respectively
            if (scores[2] > scores[3])
            {
                playerScoreTexts[2].color = secondPlaceColor;
                playerScoreTexts[3].color = thirdPlaceColor;
            }

            // If the third score is tied with the fourth score, set their text colors to second place colors
            else if (scores[2] == scores[3])
            {
                playerScoreTexts[2].color = secondPlaceColor;
                playerScoreTexts[3].color = secondPlaceColor;
            }
        }

        // Otherwise, there are 3 winners set the last text color to second place color
        else if (winnersPassed == 3)
        {
            playerScoreTexts[3].color = secondPlaceColor;
        }

        // Update the rest of the scores for the non-winners
        for (int i = winnersPassed; i < scores.Count; i++)
        {
            if (scores[i] == gameplay.players[0].Score && !scoreUpdated[0])
            {
                playerScoreTexts[i].text = "Player Score: " + scores[i];
                scoreUpdated[0] = true;
            }

            else if (scores[i] == gameplay.players[1].Score && !scoreUpdated[1])
            {
                playerScoreTexts[i].text = $"CPU{1} Score: " + scores[i];
                scoreUpdated[1] = true;
            }

            else if (scores[i] == gameplay.players[2].Score && !scoreUpdated[2])
            {
                playerScoreTexts[i].text = $"CPU{2} Score: " + scores[i];
                scoreUpdated[2] = true;
            }

            else if (scores[i] == gameplay.players[3].Score && !scoreUpdated[3])
            {
                playerScoreTexts[i].text = $"CPU{3} Score: " + scores[i];
                scoreUpdated[3] = true;
            }
        }
    }

    private void OnPlayAgainClicked()
    {
        GameManager.Instance.UIManager.ClearStack();
        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<StartState>();
    }

    private void OnQuitClicked()
    {
        GameManager.Instance.Gameplay.ClearBoard();
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.StateMachine.TransitionToScene<MainMenuState>("MainMenu");
    }
}
