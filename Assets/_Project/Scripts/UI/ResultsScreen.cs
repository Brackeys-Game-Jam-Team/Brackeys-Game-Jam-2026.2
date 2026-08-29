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

        // Print all the winners text with player's names, score and their actual colors
        for (int i = 0; i < gameplay.Winners.Count; i++)
        {
            playerScoreTexts[i].text = $"{gameplay.Winners[i].Name} Score: " + gameplay.Winners[i].Score;
            playerScoreTexts[i].color = gameplay.Winners[i].Color;

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
        List<bool> scoreUpdated = new List<bool> { false, false, false, false };

        // Update the rest of the scores for the non-winners
        for (int i = winnersPassed; i < scores.Count; i++)
        {
            if (scores[i] == gameplay.players[0].Score && !scoreUpdated[0])
            {
                playerScoreTexts[i].text = $"{gameplay.players[0].Name} Score: " + scores[i];
                playerScoreTexts[i].color = gameplay.players[0].Color;
                scoreUpdated[0] = true;
            }

            else if (scores[i] == gameplay.players[1].Score && !scoreUpdated[1])
            {
                playerScoreTexts[i].text = $"{gameplay.players[1].Name} Score: " + scores[i];
                playerScoreTexts[i].color = gameplay.players[1].Color;
                scoreUpdated[1] = true;
            }

            else if (scores[i] == gameplay.players[2].Score && !scoreUpdated[2])
            {
                playerScoreTexts[i].text = $"{gameplay.players[2].Name} Score: " + scores[i];
                playerScoreTexts[i].color = gameplay.players[2].Color;
                scoreUpdated[2] = true;
            }

            else if (scores[i] == gameplay.players[3].Score && !scoreUpdated[3])
            {
                playerScoreTexts[i].text = $"{gameplay.players[3].Name} Score: " + scores[i];
                playerScoreTexts[i].color = gameplay.players[3].Color;
                scoreUpdated[3] = true;
            }
        }
    }

    private void OnPlayAgainClicked()
    {
        GameManager.Instance.AudioManager.PlaySFX("ButtonClick2");
        GameManager.Instance.UIManager.ClearStack();
        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<StartState>();
    }

    private void OnQuitClicked()
    {
        GameManager.Instance.AudioManager.PlaySFX("ButtonClick");
        GameManager.Instance.Gameplay.ClearBoard();
        GameManager.Instance.UIManager.ClearStack();
        GameManager.Instance.StateMachine.TransitionToScene<MainMenuState>("MainMenu");
    }
}
