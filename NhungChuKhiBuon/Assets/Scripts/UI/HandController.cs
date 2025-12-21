using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public CardUI cardPrefab;
    public Transform handRoot;
    public TeamDeckManager deckManager;

    private List<CardUI> cardsInHand = new List<CardUI>();

    private bool hasDrawnInitialHand = false;

    void Start()
    {
        if (deckManager == null)
        {
            return;
        }

        // Đăng ký lắng nghe event khởi tạo deck
        deckManager.OnDeckInitialized.AddListener(OnDeckReady);

        // Nếu deck đã khởi tạo rồi (trường hợp Start chạy sau), rút bài ngay
        if (deckManager.IsInitialized())
        {
            OnDeckReady();
        }
    }

    private void OnDeckReady()
    {
        if (!hasDrawnInitialHand)
        {
            DrawNewHand();
            hasDrawnInitialHand = true;
        }
    }

    private void OnDestroy()
    {
        // Cleanup listener
        if (deckManager != null)
        {
            deckManager.OnDeckInitialized.RemoveListener(OnDeckReady);
        }
    }

    public void DrawNewHand()
    {
        if (deckManager == null)
        {
            return;
        }

        // Rút 8 lá từ DrawPile
        List<CardData> newCards = deckManager.DrawCards(8);

        // Spawn các lá bài lên hand
        foreach (CardData card in newCards)
        {
            PlayerCharacter owner = deckManager.GetOwnerOfCard(card);
            SpawnCard(card, owner);
        }
    }

    void SpawnCard(CardData card, PlayerCharacter owner)
    {
        CardUI ui = Instantiate(cardPrefab, handRoot);
        ui.Setup(card, owner, this);
        cardsInHand.Add(ui);
    }

    // Xóa card khỏi danh sách hand (gọi khi card được thêm vào queue)
    public void RemoveCardFromHand(CardUI cardUI)
    {
        if (cardsInHand.Contains(cardUI))
        {
            cardsInHand.Remove(cardUI);
        }
    }

    // Đưa card vào DiscardPile (gọi từ CardActionQueue sau khi animation xong)
    public void AddToDiscardPile(CardData cardData)
    {
        if (deckManager != null)
        {
            deckManager.AddToDiscard(cardData);
        }
    }

    // Discard toàn bộ hand hiện tại (chỉ discard những card chưa được queue)
    public void DiscardCurrentHand()
    {
        List<CardData> cardsToDiscard = new List<CardData>();
        List<CardUI> cardsToDestroy = new List<CardUI>();

        // Lấy tất cả CardData từ hand (chỉ những card chưa queued)
        foreach (CardUI cardUI in cardsInHand)
        {
            // Kiểm tra card đã được queue chưa
            if (!cardUI.IsQueued())
            {
                CardData cardData = cardUI.GetCardData();
                if (cardData != null)
                {
                    cardsToDiscard.Add(cardData);
                    cardsToDestroy.Add(cardUI);
                }
            }
        }

        // Đưa vào DiscardPile
        if (deckManager != null)
        {
            deckManager.AddToDiscard(cardsToDiscard);
        }

        // Hủy tất cả UI cards chưa được queue
        foreach (CardUI cardUI in cardsToDestroy)
        {
            Destroy(cardUI.gameObject);
        }

        // Clear danh sách (cards đã queue sẽ tự destroy khi xử lý xong)
        cardsInHand.Clear();
    }

    public int GetHandCount()
    {
        return cardsInHand.Count;
    }
}