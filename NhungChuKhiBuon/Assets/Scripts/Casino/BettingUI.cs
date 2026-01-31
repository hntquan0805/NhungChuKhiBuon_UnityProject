using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BettingUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI coinDisplayText;
    public Button bet25Button;
    public Button bet50Button;
    public Button bet75Button;
    public Button bet100Button;

    void Start()
    {
        questionText.text = "Hãy chọn mức cược!";
        UpdateCoinDisplay();

        // Gán sự kiện cho các nút với phần trăm
        bet25Button.onClick.AddListener(() => SelectBetPercentage(0.25f));
        bet50Button.onClick.AddListener(() => SelectBetPercentage(0.5f));
        bet75Button.onClick.AddListener(() => SelectBetPercentage(0.75f));
        bet100Button.onClick.AddListener(() => SelectBetPercentage(1.0f));

        // Cập nhật text cho các nút
        UpdateButtonTexts();
    }

    void UpdateCoinDisplay()
    {
        coinDisplayText.text = $"{MenuManager.Instance.PlayerCoins}";
    }

    void UpdateButtonTexts()
    {
        int totalCoins = MenuManager.Instance.PlayerCoins;

        bet25Button.GetComponentInChildren<TextMeshProUGUI>().text = $"25%\n({Mathf.FloorToInt(totalCoins * 0.25f)})";
        bet50Button.GetComponentInChildren<TextMeshProUGUI>().text = $"50%\n({Mathf.FloorToInt(totalCoins * 0.5f)})";
        bet75Button.GetComponentInChildren<TextMeshProUGUI>().text = $"75%\n({Mathf.FloorToInt(totalCoins * 0.75f)})";
        bet100Button.GetComponentInChildren<TextMeshProUGUI>().text = $"100%\n({totalCoins})";
    }

    void SelectBetPercentage(float percentage)
    {
        // Phát âm thanh khi bấm nút
        if (AudioCasinoManager.Instance != null)
        {
            AudioCasinoManager.Instance.PlayButtonClick();
        }

        int totalCoins = MenuManager.Instance.PlayerCoins;
        int betAmount = Mathf.FloorToInt(totalCoins * percentage);

        // Kiểm tra nếu người chơi có đủ coin
        if (betAmount > 0 && betAmount <= totalCoins)
        {
            MenuManager.Instance.CurrentBet = betAmount;
            MenuManager.Instance.SpendCoins(betAmount);
            MenuManager.Instance.LoadScene("Casino");
        }
        else
        {
            Debug.Log("Không đủ coin!");

            // Phát âm thanh lỗi
            if (AudioCasinoManager.Instance != null)
            {
                AudioCasinoManager.Instance.PlayLose();
            }

            // Thông báo lỗi UI
            questionText.text = "Không đủ coin! Hãy chọn mức khác.";
            Invoke(nameof(ResetQuestionText), 2f);
        }
    }

    void ResetQuestionText()
    {
        questionText.text = "Hãy chọn mức cược!";
    }
}