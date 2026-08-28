using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Emotion
{
    Idle,
    Angry,
    Sad,
    Happy
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
    [field: SerializeField] public int Id { get; private set; }

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
        //Id = id;
        IsHuman = isHuman;
        Score = 0;
        StartCoroutine(SetEmotion(Emotion.Idle));
    }

    public override string ToString()
    {
        return $"Player {Id}{(IsHuman ? " (You)" : "")}";
    }

    public IEnumerator SetEmotion(Emotion emotion)
    {
        if (spriteRenderer == null)
            yield break;

        if (emotion == Emotion.Idle)
            yield break;

        spriteRenderer.sprite = visuals[emotion].Item1;
        GameManager.Instance.AudioManager.PlayVoice(visuals[emotion].Item2);
        yield return new WaitForSecondsRealtime(.5f);
        spriteRenderer.sprite = visuals[Emotion.Idle].Item1;
    }
}