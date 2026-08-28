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
    }

    [SerializeField] private List<EmotionData> emotionSprites;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [field: SerializeField] public int Id { get; private set; }

    private Dictionary<Emotion, Sprite> visuals;

    public int Score { get; set; }
    public bool IsHuman { get; private set; }

    private void Awake()
    {
        visuals = emotionSprites.ToDictionary(item => item.value, item => item.sprite);
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

        spriteRenderer.sprite = visuals[emotion];

        if (emotion == Emotion.Idle)
            yield break;

        var am = GameManager.Instance.AudioManager;

        switch (emotion)
        {
            case Emotion.Angry:
                //am.PlayVoice("");
                //Angry voice here
                break;
            case Emotion.Sad:
                //Sad voice here
                break;
            case Emotion.Happy:
                //Happy voice here
                break;
        }

        yield return new WaitForSecondsRealtime(.5f);
        spriteRenderer.sprite = visuals[Emotion.Idle];
    }
}