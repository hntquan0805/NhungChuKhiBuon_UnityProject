using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CasinoMangaer : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites; // 6 loại bài
    public Sprite cardBackSprite;
    public Transform cardContainer;

    [Header("UI Elements")]
    public Button revealButton;
    public Button discardButton;
    public Button swapButton; // NÚT MỚI
    public Button endButton;
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI messageText;

    private List<CasinoCards> allCards = new List<CasinoCards>();
    private CasinoCards selectedCardToDiscard = null;
    private CasinoCards selectedCardToSwap = null; // BIẾN MỚI
    private bool hasDiscarded = false;
    private bool hasSwapped = false; // BIẾN MỚI
    public int discardCost = 500;
    public int swapCost = 300; // CHI PHÍ ĐỔI BÀI

    void Start()
    {
        SetupGame();
        UpdateUI();

        revealButton.onClick.AddListener(OnRevealButtonClicked);
        discardButton.onClick.AddListener(OnDiscardButtonClicked);
        swapButton.onClick.AddListener(OnSwapButtonClicked); // THÊM LISTENER
        endButton.onClick.AddListener(OnEndButtonClicked);

        discardButton.interactable = false;
        swapButton.interactable = false; // VÔ HIỆU HÓA BAN ĐẦU
    }

    void SetupGame()
    {
        // Tạo 6 lá bài
        CasinoCardType[] cardTypes = GenerateRandomCards();

        // VỊ TRÍ 4 LÁ ÚP (Ở GIỮA MÀN HÌNH)
        Vector3[] hiddenCardPositions = new Vector3[]
        {
            new Vector3(-9f, 2.5f, 0),   // Lá úp 1
            new Vector3(-3f, 2.5f, 0),   // Lá úp 2
            new Vector3(3f, 2.5f, 0),    // Lá úp 3
            new Vector3(9f, 2.5f, 0)     // Lá úp 4
        };

        // VỊ TRÍ 2 LÁ NGƯỜI CHƠI (Ở DƯỚI MÀN HÌNH)
        Vector3[] playerCardPositions = new Vector3[]
        {
            new Vector3(-3f, -4.5f, 0),  // Lá người chơi 1
            new Vector3(3f, -4.5f, 0)    // Lá người chơi 2
        };

        int hiddenIndex = 0;
        int playerIndex = 0;

        for (int i = 0; i < 6; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);

            CasinoCards card = cardObj.GetComponent<CasinoCards>();
            bool isPlayerCard = i < 2; // 2 lá đầu là của người chơi

            // Đặt vị trí tùy theo loại bài
            if (isPlayerCard)
            {
                cardObj.transform.position = playerCardPositions[playerIndex];
                playerIndex++;
            }
            else
            {
                cardObj.transform.position = hiddenCardPositions[hiddenIndex];
                hiddenIndex++;
            }

            card.Initialize(cardTypes[i], cardSprites[(int)cardTypes[i]], cardBackSprite, isPlayerCard);
            allCards.Add(card);
        }
    }

    CasinoCardType[] GenerateRandomCards()
    {
        CasinoCardType[] cards = new CasinoCardType[6];
        for (int i = 0; i < 6; i++)
        {
            cards[i] = (CasinoCardType)Random.Range(0, 6);
        }
        return cards;
    }

    void UpdateUI()
    {
        betAmountText.text = $"Cược: {MenuManager.Instance.CurrentBet}";
        coinText.text = $"Coin: {MenuManager.Instance.PlayerCoins}";
    }

    public void OnRevealButtonClicked()
    {
        // Tìm lá bài úp đầu tiên
        CasinoCards cardToReveal = allCards.FirstOrDefault(c => !c.isRevealed && !c.isPlayerCard);

        if (cardToReveal != null)
        {
            cardToReveal.Reveal();
            messageText.text = "Đã mở bài.";
        }
        else
        {
            messageText.text = "Không còn bài để mở!";
        }
    }

    public void OnCardClicked(CasinoCards card)
    {
        if (!card.isRevealed || card.isDiscarded)
        {
            return;
        }

        // Nếu click vào lá đang được chọn -> bỏ chọn
        if (selectedCardToSwap == card || selectedCardToDiscard == card)
        {
            card.isSelected = false;
            card.UpdateCardColor();

            selectedCardToSwap = null;
            selectedCardToDiscard = null;
            swapButton.interactable = false;
            discardButton.interactable = false;

            Debug.Log("Bỏ chọn card");
            return;
        }

        // Bỏ chọn lá cũ (nếu có)
        if (selectedCardToSwap != null)
        {
            selectedCardToSwap.isSelected = false;
            selectedCardToSwap.UpdateCardColor();
        }
        if (selectedCardToDiscard != null)
        {
            selectedCardToDiscard.isSelected = false;
            selectedCardToDiscard.UpdateCardColor();
        }

        // Chọn lá mới
        card.isSelected = true;
        card.selectedColor = Color.cyan;
        card.UpdateCardColor();

        // Gán cho cả 2 biến (để cả 2 nút đều biết lá nào được chọn)
        selectedCardToSwap = card;
        selectedCardToDiscard = card;

        // Bật các nút tùy theo chức năng nào chưa dùng
        swapButton.interactable = !hasSwapped;
        discardButton.interactable = !hasDiscarded;

        Debug.Log($"Chọn card - Swap available: {!hasSwapped}, Discard available: {!hasDiscarded}");
    }

    public void OnDiscardButtonClicked()
    {
        if (!hasDiscarded && selectedCardToDiscard != null)
        {
            if (MenuManager.Instance.SpendCoins(discardCost))
            {
                selectedCardToDiscard.isDiscarded = true;
                selectedCardToDiscard.isSelected = false;
                selectedCardToDiscard.UpdateCardColor();

                hasDiscarded = true;
                discardButton.interactable = false;

                // Reset selection
                selectedCardToDiscard = null;
                selectedCardToSwap = null;
                swapButton.interactable = false;

                messageText.text = $"Đã bỏ bài! Trừ {discardCost} coin.";
                UpdateUI();
            }
            else
            {
                messageText.text = "Không đủ coin để bỏ bài!";
            }
        }
    }

    // HÀM MỚI: XỬ LÝ ĐỔI BÀI
    public void OnSwapButtonClicked()
    {
        if (!hasSwapped && selectedCardToSwap != null)
        {
            if (MenuManager.Instance.SpendCoins(swapCost))
            {
                CasinoCardType oldType = selectedCardToSwap.cardType;
                CasinoCardType newType;

                // Tạo loại bài mới khác với loại hiện tại
                do
                {
                    newType = (CasinoCardType)Random.Range(0, 6);
                } while (newType == oldType);

                // Cập nhật lá bài
                selectedCardToSwap.cardType = newType;
                selectedCardToSwap.GetComponent<SpriteRenderer>().sprite = cardSprites[(int)newType];
                selectedCardToSwap.isSelected = false;
                selectedCardToSwap.UpdateCardColor();

                hasSwapped = true;
                hasDiscarded = true;
                swapButton.interactable = false;
                discardButton.interactable = false;

                if (selectedCardToSwap != null)
                {
                    selectedCardToSwap.isSelected = false;
                    selectedCardToSwap.UpdateCardColor();
                }

                // Reset selection
                selectedCardToSwap = null;
                selectedCardToDiscard = null;


                messageText.text = $"Đã đổi bài! Trừ {swapCost} coin.";
                UpdateUI();
            }
            else
            {
                messageText.text = "Không đủ coin để đổi bài!";
            }
        }
    }

    public void OnEndButtonClicked()
    {
        CalculateResult();
    }

    void CalculateResult()
    {
        // Lấy tất cả lá đã mở và không bị bỏ
        List<CasinoCards> activeCards = allCards.Where(c => c.isRevealed && !c.isDiscarded).ToList();

        int totalCards = activeCards.Count;
        int multiplier = 0;
        string resultMessage = "";

        // Trường hợp đặc biệt: Chỉ còn 1 lá
        if (totalCards == 1)
        {
            int returnAmount = Mathf.FloorToInt(MenuManager.Instance.CurrentBet * 0.2f);
            MenuManager.Instance.AddCoins(returnAmount);
            messageText.text = $"Chỉ còn 1 lá! Hoàn lại 20%: {returnAmount} coin.";
            UpdateUI();
            Invoke("ReturnToBetting", 3f);
            return;
        }

        // Đếm số lượng từng loại bài
        Dictionary<CasinoCardType, int> cardCounts = new Dictionary<CasinoCardType, int>();
        foreach (CasinoCards card in activeCards)
        {
            if (!cardCounts.ContainsKey(card.cardType))
                cardCounts[card.cardType] = 0;
            cardCounts[card.cardType]++;
        }

        // Kiểm tra có lá lẻ không (nhiều hơn 1 loại bài)
        if (cardCounts.Count > 1)
        {
            multiplier = 0;
            resultMessage = "Có lá khác biệt! Mất toàn bộ cược!";
        }
        else
        {
            // Tất cả các lá giống nhau
            int matchingCards = cardCounts.First().Value;

            switch (matchingCards)
            {
                case 2:
                    multiplier = 2;
                    resultMessage = "2 lá giống nhau! x2";
                    break;
                case 3:
                    multiplier = 5;
                    resultMessage = "3 lá giống nhau! x5";
                    break;
                case 4:
                    multiplier = 10;
                    resultMessage = "4 lá giống nhau! x10";
                    break;
                case 5:
                    multiplier = 20;
                    resultMessage = "5 lá giống nhau! x20";
                    break;
                case 6:
                    multiplier = 50;
                    resultMessage = "6 lá giống nhau! x50";
                    break;
            }
        }

        int winAmount = MenuManager.Instance.CurrentBet * multiplier;

        if (multiplier > 0)
        {
            MenuManager.Instance.AddCoins(winAmount);
            messageText.text = $"{resultMessage}\nThắng: {winAmount} coin!";
        }
        else
        {
            messageText.text = $"{resultMessage}\nMất: {MenuManager.Instance.CurrentBet} coin!";
        }

        UpdateUI();

        // Quay lại betting scene sau 3 giây
        Invoke("ReturnToBetting", 3f);
    }

    void ReturnToBetting()
    {
        MenuManager.Instance.LoadScene("Map");
    }
}