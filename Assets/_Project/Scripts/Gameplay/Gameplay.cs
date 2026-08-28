using System.Collections;
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
        players = players.OrderBy(p => p.Id).ToList();

        for (int i = 0; i < players.Count; i++)
            players[i].Initialize(isHuman: i == 0);

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

    public void ClearBoard()
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
        StartCoroutine(ResolveTurn());
    }

    private IEnumerator ResolveTurn()
    {
        if (selections == null || selections.Count == 0)
        {
            Debug.LogError("Selections are not valid");
            yield break;
        }

        var groupedByCard = selections.GroupBy(kvp => kvp.Value).ToDictionary(g => g.Key, g => g.Select(kvp => kvp.Key).ToList());

        foreach (var (card, pickers) in groupedByCard)
        {
            if (card.Value == CardValue.Special)
                ResolveSpecialCard(pickers);

            else
                ResolveValueCard(card.Value, pickers);
        }

        var pickedValueCards = selections.Values.Where(c => c.Value != CardValue.Special).Select(c => c.Value).ToList();
        CardValue? lowestPickedValue = null;

        if (pickedValueCards.Count > 0)
            lowestPickedValue = pickedValueCards.Max();

        foreach (var player in players)
        {
            Card chosenCard = selections[player];
            int grabberCount = groupedByCard[chosenCard].Count;
            bool isSolo = grabberCount == 1;
            bool isShared = grabberCount > 1;
            bool gotLowestCard = lowestPickedValue.HasValue && chosenCard.Value == lowestPickedValue.Value;

            // Can be changed later
            if (isShared)
                yield return player.SetEmotion(Emotion.Angry);

            else if (gotLowestCard)
                yield return player.SetEmotion(Emotion.Sad);
        }

        // Update UI texts outside of the for loop above
        ScoreTurnCountOverlay scoreTurnCountOverlayObject = FindAnyObjectByType<ScoreTurnCountOverlay>();
        if (scoreTurnCountOverlayObject != null)
            scoreTurnCountOverlayObject.UpdatePlayerScoreText();

        AIScoreOverlay aiScoreOverlayObject = FindAnyObjectByType<AIScoreOverlay>();
        if (aiScoreOverlayObject != null)
            aiScoreOverlayObject.UpdateAIScoreTexts();

        foreach (var card in groupedByCard.Keys)
        {
            activeCards.Remove(card);
            Destroy(card.gameObject);
        }

        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<CheckConditionState>();
    }

    // Call target.SetEmotion(Emotion.Happy); for any player who got the solo pick, and angry for those who share or got the lowest value card
    private void ResolveValueCard(CardValue value, List<Player> pickers)
    {
        int totalValue = GetPointValue(value);
        int share = Mathf.Max(totalValue / pickers.Count, 1);

        foreach (var player in pickers)
            player.Score += share;
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

        StartCoroutine(StealPoints(picker, leaders, specialAmount));
    }

    private IEnumerator StealPoints(Player picker, List<Player> targets, int totalAmount)
    {
        if (targets.Count == 0)
            yield break;

        // Distribute the cost
        int baseSteal = totalAmount / targets.Count;
        int remainder = totalAmount % targets.Count;

        foreach (var target in targets)
        {
            int amount = baseSteal + (remainder > 0 ? 1 : 0);

            if (remainder > 0)
                remainder--;

            target.Score -= amount;

            yield return target.SetEmotion(Emotion.Sad);
        }

        picker.Score += totalAmount;
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