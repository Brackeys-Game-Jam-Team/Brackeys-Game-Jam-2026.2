using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Emotion
{
    Idle,
    Angry,
    Sad,
    Happy,
    Laugh
}

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct EmotionData
    {
        public Emotion value;
        public Sprite sprite;
        public string voice;
    }

    [SerializeField] private List<EmotionData> emotionSprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }

    private Dictionary<Emotion, (Sprite, string)> visuals;

    public int Score { get; set; }
    public bool IsHuman { get; private set; }

    private void Awake()
    {
        visuals = emotionSprites.ToDictionary(item => item.value, item => (item.sprite, item.voice));
        GameManager.Instance.Players.Register(this);
    }

    public void Initialize(bool isHuman)
    {
        IsHuman = isHuman;
        Score = 0;
        StartCoroutine(SetEmotion(Emotion.Idle));
    }

    public override string ToString()
    {
        return $"{Name}{(IsHuman ? $" (You)" : "")}";
    }

    public IEnumerator SetEmotion(Emotion emotion)
    {
        if (spriteRenderer == null)
            yield break;

        if (emotion == Emotion.Idle)
            yield break;

        spriteRenderer.sprite = visuals[emotion].Item1;

        if (Random.value < .5f)
            GameManager.Instance.AudioManager.PlayVoice(visuals[emotion].Item2);

        yield return new WaitForSecondsRealtime(.5f);
        spriteRenderer.sprite = visuals[Emotion.Idle].Item1;
    }

    public void Laugh()
    {
        if (visuals == null || visuals.Count <= 0)
            return;

        GameManager.Instance.AudioManager.PlayVoice(visuals[Emotion.Laugh].Item2);
    }
}