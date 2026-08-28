using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CardValue
{
    One,
    Three,
    Five,
    Ten,
    Special
}

public class Gameplay : MonoBehaviour
{
    [System.Serializable]
    public struct CardVisualData
    {
        public CardValue value;
        public Sprite sprite;
    }

    [SerializeField] private Transform cardContainer;
    [SerializeField] private Card cardPrefab;
    [SerializeField] private List<CardVisualData> cardVisuals;

    [Header("Grid")]
    [SerializeField] private int totalCards = 12;
    [SerializeField] private int gridColumns = 3;
    [SerializeField] private Vector2 spacing = new(2f, 3f);

    public int turnCount;
    private readonly List<Card> activeCards = new();
    public List<Player> players = new();
    private Dictionary<CardValue, Sprite> visuals;
    private Dictionary<Player, Card> selections;

    private const int WINNING_SCORE = 20;

    public List<Player> Winners { get; private set; } = new();
    private Player HumanPlayer => players[0];

    private void Awake()
    {
        visuals = cardVisuals.ToDictionary(item => item.value, item => item.sprite);
    }

    // Called during the StartState, generates cards and spawns them to the map
    public void StartGame()
    {
        players = GameManager.Instance.Players.GetAll<Player>();

        for (int i = 0; i < players.Count; i++)
        {
            players[i].Initialize(i + 1, isHuman: i == 0);
            Debug.Log(players[i].name);
        }


        turnCount = 0;
        GenerateCards();
    }

    private void GenerateCards()
    {
        ClearBoard();
        var deck = GenerateDeck(totalCards);

        int rows = Mathf.CeilToInt((float)deck.Count / gridColumns);

        float gridWidth = (gridColumns - 1) * spacing.x;
        float gridHeight = (rows - 1) * spacing.y;

        Vector3 centerOffset = new(-gridWidth * 0.5f, gridHeight * 0.5f, 0f);

        for (int i = 0; i < deck.Count; i++)
        {
            int col = i % gridColumns;
            int row = i / gridColumns;
            Vector3 position = new Vector3(col * spacing.x, -row * spacing.y, 0f) + centerOffset;

            CardValue value = deck[i];
            Sprite sprite = visuals.GetValueOrDefault(value);

            Card cardInstance = Instantiate(cardPrefab, cardContainer);
            cardInstance.transform.localPosition = position;
            cardInstance.Initialize(value, sprite, OnCardSelected);
            activeCards.Add(cardInstance);
        }
    }

    private void ClearBoard()
    {
        foreach (var card in activeCards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        activeCards.Clear();
    }

    private List<CardValue> GenerateDeck(int size)
    {
        if (size < 1)
        {
            Debug.LogError("Grid size must be at least 1.");
            return default;
        }

        int r = size - 1;
        int one = Mathf.RoundToInt(r * 30f / 90f);
        int three = Mathf.RoundToInt(r * 30f / 90f);
        int five = Mathf.RoundToInt(r * 20f / 90f);
        int ten = r - three - one - five;

        var deck = new List<CardValue>(size) { CardValue.Special };
        deck.AddRange(Enumerable.Repeat(CardValue.One, one));
        deck.AddRange(Enumerable.Repeat(CardValue.Three, three));
        deck.AddRange(Enumerable.Repeat(CardValue.Five, five));
        deck.AddRange(Enumerable.Repeat(CardValue.Ten, ten));

        // Fisher-Yates Shuffle
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }

        return deck;
    }

    // Called when the player selects a card, and randomly sets choices for all other "players"
    private void OnCardSelected(Card card)
    {
        selections = new Dictionary<Player, Card>
        {
            { HumanPlayer, card }
        };

        for (int i = 1; i < players.Count; i++)
        {
            Card randomCard = activeCards[Random.Range(0, activeCards.Count)];
            selections.Add(players[i], randomCard);
        }

        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<CompareState>();

        //ResolveRound();
    }

    // Called during CompareState, applies score, special card, updates the score
    public void ResolveRound()
    {
        if (selections == null || selections.Count == 0)
        {
            Debug.LogError("Selections are not valid");
            return;
        }

        var groupedByCard = selections.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.Select(kvp => kvp.Key).ToList());

        foreach (var (card, pickers) in groupedByCard)
        {
            if (card.Value == CardValue.Special)
                ResolveSpecialCard(pickers);

            else
                ResolveValueCard(card.Value, pickers);
        }

        // Update score count UI here
        foreach (var p in players)
        {
            Debug.Log($"{p}: {p.Score}");
        }

        // Update UI texts outside of the for loop above
        ScoreTurnCountOverlay scoreTurnCountOverlayObject = FindAnyObjectByType<ScoreTurnCountOverlay>();
        if (scoreTurnCountOverlayObject != null) scoreTurnCountOverlayObject.UpdatePlayerScoreText();

        AIScoreOverlay aiScoreOverlayObject = FindAnyObjectByType<AIScoreOverlay>();
        if (aiScoreOverlayObject != null) aiScoreOverlayObject.UpdateAIScoreTexts();

        foreach (var card in groupedByCard.Keys)
        {
            activeCards.Remove(card);
            Destroy(card.gameObject);
        }

        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<CheckConditionState>();


        //if (CheckGameCondition())
        //{
        //    Debug.Log("Game Cycle Ended.");
        //    return;
        //}
    }

    private void ResolveValueCard(CardValue value, List<Player> pickers)
    {
        int totalValue = GetPointValue(value);
        int share = Mathf.Max(totalValue / pickers.Count, 1);

        foreach (var player in pickers)
            player.Score += share;

        Debug.Log($"{value} card ({totalValue}pts) split between {pickers.Count} player(s) > {share} each.");
    }

    private void ResolveSpecialCard(List<Player> pickers)
    {
        const int specialAmount = 5;

        if (pickers.Count != 1)
        {
            Debug.Log($"Special card picked by {pickers.Count} players. No effect.");
            return;
        }

        Player picker = pickers[0];
        int leadScore = players.Max(p => p.Score);
        List<Player> leaders = players.Where(p => p.Score == leadScore).ToList();

        // TODO: What happens if no one has any points?
        if (leaders.Count == players.Count)
        {
            return;
        }

        // Picker is the sole lead so they just gain 5
        if (leaders.Count == 1 && leaders[0] == picker)
        {
            picker.Score += specialAmount;
            Debug.Log($"{picker} is sole leader > gains {specialAmount} pts.");
            return;
        }

        // Picker is one of multiple leaders, so they steal from OTHER leaders
        if (leaders.Contains(picker))
            leaders.Remove(picker);

        StealPoints(picker, leaders, specialAmount);
    }

    private void StealPoints(Player picker, List<Player> targets, int totalAmount)
    {
        if (targets.Count == 0)
            return;

        // Distribute the cost
        int baseSteal = totalAmount / targets.Count;
        int remainder = totalAmount % targets.Count;

        foreach (var target in targets)
        {
            int amount = baseSteal + (remainder > 0 ? 1 : 0);

            if (remainder > 0)
                remainder--;

            target.Score -= amount;
            Debug.Log($"{picker} steals {amount} pts from {target}.");
        }

        picker.Score += totalAmount;
        Debug.Log($"{picker} gains {totalAmount} pts from special card.");
    }
    
    // Called during the CheckConditionState to determine whether the game should continue or end
    public bool CheckGameCondition()
    {
        bool outOfCards = activeCards.Count == 0;
        bool scoreReached = players.Where(p => p.Score >= WINNING_SCORE).ToList().Count > 0;
        return outOfCards || scoreReached;
    }

    // Considering modifying this for the EndState
    public void AnnounceWinners()
    {
        int winningScore = players.Max(p => p.Score);
        Winners = players.Where(p => p.Score == winningScore).ToList();
        string winnerNames = string.Join(", ", Winners.Select(w => w.ToString()));
        Debug.Log($"The game is over! Winner(s): {winnerNames} with {winningScore} points!");
    }

    private static int GetPointValue(CardValue value) => value switch
    {
        CardValue.One => 1,
        CardValue.Three => 3,
        CardValue.Five => 5,
        CardValue.Ten => 10,
        _ => 0
    };
}