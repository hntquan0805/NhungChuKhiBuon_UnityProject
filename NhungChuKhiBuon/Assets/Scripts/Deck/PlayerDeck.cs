using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public PlayerCharacter owner;
    [Header("Bộ bài riêng của player này (8 lá)")]
    public List<CardData> deckCards = new List<CardData>(); // Setup sẵn trong Inspector

    public List<CardData> GetDeckCards()
    {
        return new List<CardData>(deckCards); // Return copy để không modify original
    }
}