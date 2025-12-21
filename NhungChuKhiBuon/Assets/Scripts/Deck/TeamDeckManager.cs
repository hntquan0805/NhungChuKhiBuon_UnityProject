using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeamDeckManager : MonoBehaviour
{
    public PlayerTeam playerTeam;

    private List<CardData> drawPile = new List<CardData>();
    private List<CardData> discardPile = new List<CardData>();
    private Dictionary<CardData, PlayerCharacter> cardOwnerMap = new Dictionary<CardData, PlayerCharacter>();

    // Event để thông báo deck đã khởi tạo xong
    public UnityEvent OnDeckInitialized = new UnityEvent();
    private bool isInitialized = false;

    private void Start()
    {
        // Đợi 1 frame để đảm bảo PlayerTeam.Awake() đã chạy xong
        StartCoroutine(DelayedInitialize());
    }

    private System.Collections.IEnumerator DelayedInitialize()
    {
        yield return null; // Đợi 1 frame
        InitializeDeck();
    }

    void InitializeDeck()
    {
        if (playerTeam == null)
        {
            return;
        }

        if (playerTeam.players == null || playerTeam.players.Count == 0)
        {
            return;
        }

        drawPile.Clear();
        discardPile.Clear();
        cardOwnerMap.Clear();

        // Lấy hết bài từ tất cả players
        int totalCardsAdded = 0;
        foreach (var player in playerTeam.players)
        {
            PlayerDeck deck = player.GetComponent<PlayerDeck>();
            if (deck == null)
            {
                deck = player.gameObject.AddComponent<PlayerDeck>();
                deck.owner = player;
            }

            List<CardData> playerCards = deck.GetDeckCards();

            if (playerCards.Count == 0)
            {
                continue;
            }

            // Thêm vào DrawPile và ghi nhớ owner
            foreach (var card in playerCards)
            {
                if (card == null)
                {
                    continue;
                }

                drawPile.Add(card);
                cardOwnerMap[card] = player;
                totalCardsAdded++;
            }
        }

        // Xáo bài DrawPile
        ShuffleCards(drawPile);

        // Đánh dấu đã khởi tạo xong và trigger event
        isInitialized = true;
        OnDeckInitialized?.Invoke();
    }

    void ShuffleCards(List<CardData> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            CardData temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    // Rút N lá bài từ DrawPile
    public List<CardData> DrawCards(int count)
    {
        List<CardData> drawnCards = new List<CardData>();

        for (int i = 0; i < count; i++)
        {
            // Nếu DrawPile hết bài, shuffle DiscardPile vào DrawPile
            if (drawPile.Count == 0)
            {
                if (discardPile.Count == 0)
                {
                    break;
                }

                drawPile.AddRange(discardPile);
                discardPile.Clear();
                ShuffleCards(drawPile);
            }

            // Rút lá đầu tiên
            CardData card = drawPile[0];
            drawPile.RemoveAt(0);
            drawnCards.Add(card);
        }

        return drawnCards;
    }

    // Đưa card vào DiscardPile
    public void AddToDiscard(CardData card)
    {
        if (!discardPile.Contains(card))
        {
            discardPile.Add(card);
        }
    }

    // Đưa nhiều cards vào DiscardPile
    public void AddToDiscard(List<CardData> cards)
    {
        foreach (var card in cards)
        {
            AddToDiscard(card);
        }
    }

    public PlayerCharacter GetOwnerOfCard(CardData card)
    {
        if (cardOwnerMap.ContainsKey(card))
        {
            return cardOwnerMap[card];
        }

        return null;
    }

    public int GetDrawPileCount()
    {
        return drawPile.Count;
    }

    public int GetDiscardPileCount()
    {
        return discardPile.Count;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }
}