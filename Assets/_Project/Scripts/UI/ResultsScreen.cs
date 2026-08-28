using System;
using System.Collections.Generic;
using System.Linq;
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

    // DANIEL: Gameplay reference (keeps a list of players' scores and the turn count)
    private Gameplay gameplay => GameManager.Instance.Gameplay;

    private void Awake()
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

        for (int i = 0; i < gameplay.Winners.Count; i++)
        {
            // Set the player score texts depending on the player's and all the AI's scores
            if (gameplay.Winners[i].Score == gameplay.players[0].Score)
            {
                playerScoreTexts[i].text = "Player Score: " + gameplay.Winners[i].Score;
            }

            else if (gameplay.Winners[i].Score == gameplay.players[1].Score)
            {
                playerScoreTexts[i].text = $"AI {1} Score: " + gameplay.Winners[i].Score;
            }

            else if (gameplay.Winners[i].Score == gameplay.players[2].Score)
            {
                playerScoreTexts[i].text = $"AI {2} Score: " + gameplay.Winners[i].Score;
            }

            else if (gameplay.Winners[i].Score == gameplay.players[3].Score)
            {
                playerScoreTexts[i].text = $"AI {3} Score: " + gameplay.Winners[i].Score;
            }

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

        List<Color> textColors = new List<Color> { firstPlaceColor, secondPlaceColor, thirdPlaceColor, fourthPlaceColor };

        // Set text colors based on highest scores
        for (int i = 0; i < scores.Count; i++)
        {
            playerScoreTexts[i].color = textColors[i];
        }

        // Update the rest of the scores for the non-winners
        for (int i = winnersPassed; i < scores.Count; i++)
        {
            if (scores[i] == gameplay.players[0].Score)
            {
                playerScoreTexts[i].text = "Player Score: " + scores[i];
            }

            else if (scores[i] == gameplay.players[1].Score)
            {
                playerScoreTexts[i].text = $"AI {1} Score: " + scores[i];
            }

            else if (scores[i] == gameplay.players[2].Score)
            {
                playerScoreTexts[i].text = $"AI {2} Score: " + scores[i];
            }

            else if (scores[i] == gameplay.players[3].Score)
            {
                playerScoreTexts[i].text = $"AI {3} Score: " + scores[i];
            }
        }
    }

    private void OnPlayAgainClicked()
    {
        //GameManager.Instance.StateMachine.GetState<GameplayState>();
        //GameManager.Instance.StateMachine.ChangeState<StartState>();
    }

    private void OnQuitClicked()
    {
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.StateMachine.TransitionToScene<MainMenuState>("MainMenu");
    }
}
