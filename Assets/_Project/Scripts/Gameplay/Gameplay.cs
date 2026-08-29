using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private List<CardVisualData> cardVisuals;

    [Header("Grid")]
    [SerializeField] private int totalCards = 12;
    [SerializeField] private int gridColumns = 3;
    [SerializeField] private Vector2 spacing = new(2f, 3f);
    [SerializeField] private float dealDuration = 0.3f;
    [SerializeField] private float delayBetweenCards = 0.05f;

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
        StartGenerateCards();
    }

    private void StartGenerateCards()
    {
        StartCoroutine(GenerateCardsRoutine());
    }

    private IEnumerator GenerateCardsRoutine()
    {
        ClearBoard();
        var deck = GenerateDeck(totalCards);
        int rows = Mathf.CeilToInt((float)deck.Count / gridColumns);
        float gridWidth = (gridColumns - 1) * spacing.x;
        float gridHeight = (rows - 1) * spacing.y;

        Vector3 centerOffset = new(-gridWidth * 0.5f, gridHeight * 0.5f, 0f);
        Vector3 startPosition = Vector3.zero;

        for (int i = 0; i < deck.Count; i++)
        {
            int col = i % gridColumns;
            int row = i / gridColumns;
            Vector3 targetPosition = new Vector3(col * spacing.x, -row * spacing.y, 0f) + centerOffset;

            CardValue value = deck[i];
            Sprite sprite = visuals.GetValueOrDefault(value);

            Card cardInstance = Instantiate(cardPrefab, cardContainer);
            cardInstance.transform.localPosition = startPosition;
            cardInstance.Initialize(value, sprite, OnCardSelected);
            activeCards.Add(cardInstance);

            StartCoroutine(AnimateCardToPosition(cardInstance.transform, startPosition, targetPosition, dealDuration));
            yield return new WaitForSeconds(delayBetweenCards);
        }

        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<SelectState>();
    }

    private IEnumerator AnimateCardToPosition(Transform cardTransform, Vector3 start, Vector3 target, float duration)
    {
        GameManager.Instance.AudioManager.PlaySFX("CardSpread");
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (cardTransform == null)
                yield break;

            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            cardTransform.localPosition = Vector3.Lerp(start, target, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (cardTransform != null)
            cardTransform.localPosition = target;
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
            int randomIndex = Random.Range(0, i + 1);
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
        var pickedValueCards = selections.Values.Where(c => c.Value != CardValue.Special).Select(c => c.Value).ToList();
        CardValue? lowestPickedValue = pickedValueCards.Count > 0 ? pickedValueCards.Max() : null;
        List<Coroutine> activeAnimations = new();

        foreach (var player in players)
        {
            GameManager.Instance.AudioManager.PlaySFX("CardPickup");
            Card chosenCard = selections[player];
            bool isSolo = groupedByCard[chosenCard].Count == 1;
            Vector3 targetPosition = isSolo ? player.transform.position : Vector3.zero;

            if (!isSolo)
                SpawnFloatingText("SHARED SPLIT!", Vector3.zero, Color.orange);

            activeAnimations.Add(StartCoroutine(AnimateCard(chosenCard, targetPosition, 0.5f)));
        }

        foreach (var anim in activeAnimations)
            yield return anim;

        foreach (var player in players)
        {
            Card chosenCard = selections[player];
            chosenCard.transform.SetParent(null);
            bool isShared = groupedByCard[chosenCard].Count > 1;
            bool gotLowestCard = lowestPickedValue.HasValue && chosenCard.Value == lowestPickedValue.Value;

            if (isShared)
                yield return player.SetEmotion(Emotion.Angry);

            else if (gotLowestCard)
                yield return player.SetEmotion(Emotion.Sad);
        }

        foreach (var (card, pickers) in groupedByCard)
        {
            if (card.Value == CardValue.Special)
                ResolveSpecialCard(pickers);

            else
                ResolveValueCard(card.Value, pickers);
        }

        FindAnyObjectByType<ScoreTurnCountOverlay>()?.UpdatePlayerScoreText();
        FindAnyObjectByType<AIScoreOverlay>()?.UpdateAIScoreTexts();

        foreach (var card in groupedByCard.Keys)
        {
            activeCards.Remove(card);
            Destroy(card.gameObject);
        }

        var gs = GameManager.Instance.StateMachine.GetState<GameplayState>();
        gs.ChangeState<CheckConditionState>();
    }

    private IEnumerator AnimateCard(Card card, Vector3 targetPos, float duration)
    {
        Vector3 startPos = card.transform.localPosition;
        Quaternion startRot = card.transform.rotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            card.transform.SetPositionAndRotation(Vector3.Lerp(startPos, targetPos, t), Quaternion.Lerp(startRot, Quaternion.identity, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        card.transform.SetPositionAndRotation(targetPos, Quaternion.identity);
    }

    private void SpawnFloatingText(string text, Vector3 position, Color color)
    {
        if (floatingTextPrefab == null)
            return;

        GameObject popup = Instantiate(floatingTextPrefab, position, Quaternion.identity);
        var tmp = popup.GetComponentInChildren<TMP_Text>();

        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
        }

        StartCoroutine(AnimateFloatingText(popup));
    }

    private IEnumerator AnimateFloatingText(GameObject obj)
    {
        float duration = 1.2f;
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = startPos + Vector3.up * 1.5f;
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            obj.transform.position = Vector3.Lerp(startPos, targetPos, t);

            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }

    // Call target.SetEmotion(Emotion.Happy); for any player who got the solo pick, and angry for those who share or got the lowest value card
    private void ResolveValueCard(CardValue value, List<Player> pickers)
    {
        int totalValue = GetPointValue(value);
        int share = Mathf.Max(totalValue / pickers.Count, 1);

        if (pickers.Count > 1)
            GameManager.Instance.AudioManager.PlaySFX("ScoreMid");

        else
            GameManager.Instance.AudioManager.PlaySFX("ScoreFull");

        foreach (var player in pickers)
        {
            player.Score += share;
            SpawnFloatingText($"+{share}", player.transform.position, player.Color);
        }
    }

    private void ResolveSpecialCard(List<Player> pickers)
    {
        const int specialAmount = 5;

        if (pickers.Count != 1)
            return;

        Player picker = pickers[0];
        int leadScore = players.Max(p => p.Score);
        List<Player> leaders = players.Where(p => p.Score == leadScore).ToList();

        // Picker is the sole lead so they just gain 5
        if (leaders.Count == 1 && leaders[0] == picker)
        {
            picker.Score += specialAmount;
            SpawnFloatingText($"+{specialAmount}", picker.transform.position, picker.Color);
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

            target.Score = Mathf.Max(0, target.Score - amount);

            StartCoroutine(target.SetEmotion(Emotion.Sad));
            SpawnFloatingText($"-{amount}", target.transform.position, target.Color);
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

    public void AnnounceWinners()
    {
        int winningScore = players.Max(p => p.Score);
        Winners = players.Where(p => p.Score == winningScore).ToList();
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