using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RestAreaManager : MonoBehaviour
{
    public ShopManager shopManager;

    public TMP_Text coinText;

    void Start()
    {
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (coinText != null && MenuManager.Instance != null)
        {
            coinText.text = MenuManager.Instance.PlayerCoins.ToString();
        }
    }

    public void ExitToCasino()
    {
        if (shopManager != null)
        {
            shopManager.ResetShop();
        }

        SceneManager.LoadScene("Menu"); // đổi tên nếu khác
    }

    void OnEnable()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.OnCoinsChanged += UpdateCoinUI;
    }

    void OnDisable()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.OnCoinsChanged -= UpdateCoinUI;
    }

    public void UpdateCoinUI(int newCoin)
    {
        coinText.text = newCoin.ToString();
    }

}
