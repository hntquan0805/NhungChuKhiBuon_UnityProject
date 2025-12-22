using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BettingUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI coinDisplayText;
    public Button bet1000Button;
    public Button bet2000Button;
    public Button bet5000Button;
    public Button betAllButton;

    void Start()
    {
        questionText.text = "Mức cược mong muốn lần này của quý khách là?";
        UpdateCoinDisplay();

        // Gán sự kiện cho các nút
        bet1000Button.onClick.AddListener(() => SelectBet(1000));
        bet2000Button.onClick.AddListener(() => SelectBet(2000));
        bet5000Button.onClick.AddListener(() => SelectBet(5000));
        betAllButton.onClick.AddListener(() => SelectBet(MenuManager.Instance.PlayerCoins));

        // Đặt text cho các nút
        bet1000Button.GetComponentInChildren<TextMeshProUGUI>().text = "1000 coin";
        bet2000Button.GetComponentInChildren<TextMeshProUGUI>().text = "2000 coin";
        bet5000Button.GetComponentInChildren<TextMeshProUGUI>().text = "5000 coin";
        betAllButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tất tay";
    }

    void UpdateCoinDisplay()
    {
        coinDisplayText.text = $"Số coin hiện có: {MenuManager.Instance.PlayerCoins}";
    }

    void SelectBet(int amount)
    {
        if (amount <= MenuManager.Instance.PlayerCoins)
        {
            MenuManager.Instance.CurrentBet = amount;
            MenuManager.Instance.SpendCoins(amount);
            MenuManager.Instance.LoadScene("Casino");
        }
        else
        {
            Debug.Log("Không đủ coin!");
            // Có thể thêm thông báo lỗi UI ở đây
        }
    }
}