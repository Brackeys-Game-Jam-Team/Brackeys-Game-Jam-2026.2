using System;
using System.Collections.Generic;
using System.Linq;

public class Gameplay
{
    public enum CardValue
    {
        Ten,
        Five,
        Three,
        One,
        Special
    }

    public List<CardValue> Cards;

    public const int CARD_GRID_SIZE = 12;

    public void GenerateCards()
    {
        if (CARD_GRID_SIZE < 1)
            throw new ArgumentOutOfRangeException(nameof(CARD_GRID_SIZE));

        // Calculate card ratios
        int r = CARD_GRID_SIZE - 1;
        int one = (int)Math.Round(r * 30 / 90.0);
        int three = (int)Math.Round(r * 30 / 90.0);
        int five = (int)Math.Round(r * 20 / 90.0);
        int ten = r - three - one - five;

        // Build the card list
        Cards = new List<CardValue>(CARD_GRID_SIZE) { CardValue.Special };
        Cards.AddRange(Enumerable.Repeat(CardValue.Three, three));
        Cards.AddRange(Enumerable.Repeat(CardValue.One, one));
        Cards.AddRange(Enumerable.Repeat(CardValue.Five, five));
        Cards.AddRange(Enumerable.Repeat(CardValue.Ten, ten));

        // Fisher-Yates shuffle method
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            (Cards[i], Cards[randomIndex]) = (Cards[randomIndex], Cards[i]);
        }
    }

    public void RemoveCard(int index)
    {
        Cards.RemoveAt(index);
    }
}