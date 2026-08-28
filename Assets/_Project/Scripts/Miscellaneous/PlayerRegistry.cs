using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerRegistry
{
    private readonly HashSet<Player> allCharacters = new();

    public IReadOnlyCollection<Player> All => allCharacters;

    public void Register(Player character)
    {
        allCharacters.Add(character);
    }

    public void Unregister(Player character)
    {
        allCharacters.Remove(character);
    }

    public void ClearCharacters()
    {
        allCharacters.Clear();
    }

    public List<T> GetAll<T>() where T : Player
    {
        return allCharacters.OfType<T>().ToList();
    }

    public T GetClosest<T>(Vector3 position, float maxRange = float.MaxValue) where T : Player
    {
        T closest = null;
        float closestDist = maxRange;

        foreach (var character in allCharacters)
        {
            if (character is T typed)
            {
                float dist = Vector3.Distance(position, character.transform.position);

                if (dist < closestDist)
                {
                    closest = typed;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }
}