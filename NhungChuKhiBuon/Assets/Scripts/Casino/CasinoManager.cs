using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CasinoMangaer : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public Sprite[] cardSprites; // 6 loại bài
    public Sprite cardBackSprite;
    public Transform cardContainer;

    [Header("UI")]
    public TextMeshProUGUI betAmountText;
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI messageText;

    private List<CasinoCards> cards = new List<CasinoCards>();
    private bool hasChosen = false;

    void Start()
    {
        SetupGame();
        UpdateUI();
    }

    void SetupGame()
    {
        CasinoCardType[] cardTypes = GenerateRandomCards();

        Vector3[] positions =
        {
            new Vector3(-9f, 0f, 0),
            new Vector3(-5f, 0f, 0),
            new Vector3(-1f, 0f, 0),
            new Vector3(3f, 0f, 0),
            new Vector3(7f, 0f, 0),
            new Vector3(11f, 0f, 0),
        };

        for (int i = 0; i < 6; i++)
        {
            GameObject obj = Instantiate(cardPrefab, cardContainer);
            obj.transform.position = positions[i];

            CasinoCards card = obj.GetComponent<CasinoCards>();
            card.Initialize(cardTypes[i], cardSprites[(int)cardTypes[i]], cardBackSprite);

            cards.Add(card);
        }

        messageText.text = "Chọn 1 lá bài!";
    }

    CasinoCardType[] GenerateRandomCards()
    {
        CasinoCardType[] result = new CasinoCardType[6];
        for (int i = 0; i < 6; i++)
            result[i] = (CasinoCardType)Random.Range(0, cardSprites.Length);

        return result;
    }

    void UpdateUI()
    {
        betAmountText.text = $"Cược: {MenuManager.Instance.CurrentBet}";
        coinText.text = $"{MenuManager.Instance.PlayerCoins}";
    }

    // Được gọi từ CasinoCards.OnMouseDown()
    public void OnCardClicked(CasinoCards card)
    {
        if (hasChosen) return;

        hasChosen = true;
        card.Reveal();

        // Delay để đợi animation lật bài hoàn thành trước khi phát âm thanh kết quả
        Invoke(nameof(DelayedResolve), 0.5f);
        currentCardType = card.cardType;
    }

    private CasinoCardType currentCardType;

    void DelayedResolve()
    {
        ResolveResult(currentCardType);
    }

    void ResolveResult(CasinoCardType type)
    {
        int bet = MenuManager.Instance.CurrentBet;
        int reward = 0;
        string msg = "";
        bool isWin = false;
        bool isBigWin = false;

        switch (type)
        {
            case CasinoCardType.Type1:
                reward = 0;
                msg = "Mất toàn bộ cược!";
                isWin = false;
                break;

            case CasinoCardType.Type2:
                reward = bet / 2;
                msg = "Mất một nửa cược!";
                isWin = false;
                break;

            case CasinoCardType.Type3:
                reward = bet;
                msg = "Hoàn tiền!";
                isWin = true;
                break;

            case CasinoCardType.Type4:
                reward = bet * 2;
                msg = "Thắng x2!";
                isWin = true;
                break;

            case CasinoCardType.Type5:
                reward = bet * 5;
                msg = "Thắng x5!";
                isWin = true;
                isBigWin = true;
                break;

            case CasinoCardType.Type6:
                reward = bet * 10;
                msg = "Thắng x10!!!";
                isWin = true;
                isBigWin = true;
                break;
        }

        // Phát âm thanh dựa trên kết quả
        if (AudioCasinoManager.Instance != null)
        {
            if (isBigWin)
            {
                AudioCasinoManager.Instance.PlayBigWin();
            }
            else if (isWin)
            {
                AudioCasinoManager.Instance.PlayWin();
            }
            else
            {
                AudioCasinoManager.Instance.PlayLose();
            }
        }

        if (reward > 0)
            MenuManager.Instance.AddCoins(reward);

        messageText.text = msg + $"\nNhận: {reward} coin";
        UpdateUI();

        Invoke(nameof(ReturnToMap), 3f);
    }

    void ReturnToMap()
    {
        // Quay về MapLv tương ứng
        if (MapProgressManager.Instance != null && MapProgressManager.Instance.HasActiveMap())
        {
            string mapScene = MapProgressManager.Instance.GetCurrentMapScene();
            MenuManager.Instance.LoadScene(mapScene);
        }
        else
        {
            MenuManager.Instance.LoadScene("Map");
        }
    }
}