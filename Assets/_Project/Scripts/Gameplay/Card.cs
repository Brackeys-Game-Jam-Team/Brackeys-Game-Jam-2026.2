using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Card : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public CardValue Value { get; private set; }
    private Action<Card> _onClickCallback;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(CardValue value, Sprite sprite, Action<Card> onClickCallback)
    {
        Value = value;
        spriteRenderer.sprite = sprite;
        _onClickCallback = onClickCallback;
    }

    private void OnMouseEnter()
    {
        Debug.Log($"Hover: {Value}");
    }

    private void OnMouseExit()
    {
        Debug.Log($"Unhover: {Value}");
    }

    private void OnMouseDown()
    {
        _onClickCallback?.Invoke(this);
    }
}