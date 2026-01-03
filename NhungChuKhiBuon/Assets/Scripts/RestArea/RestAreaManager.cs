using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RestAreaManager : MonoBehaviour
{
    public ShopManager shopManager;
    public TMP_Text coinText;

    [Header("Healing Options")]
    public int fullHealCost = 100;
    public int partialHealAmount = 50;
    public int partialHealCost = 30;

    [Header("UI Buttons (Optional)")]
    public UnityEngine.UI.Button fullHealButton;
    public UnityEngine.UI.Button partialHealButton;

    void Start()
    {
        UpdateCoinUI();

        // Setup heal buttons nếu có
        if (fullHealButton != null)
        {
            fullHealButton.onClick.AddListener(FullHealTeam);
        }

        if (partialHealButton != null)
        {
            partialHealButton.onClick.AddListener(PartialHealTeam);
        }

        // Log team status khi vào Rest Area
        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.LogTeamStatus();
        }
    }

    public void UpdateCoinUI()
    {
        if (coinText != null && MenuManager.Instance != null)
        {
            coinText.text = MenuManager.Instance.PlayerCoins.ToString();
        }
    }

    // ===== NEW: HEAL TEAM TO FULL =====
    public void FullHealTeam()
    {
        if (PersistentTeamManager.Instance == null)
        {
            Debug.LogError("[RestArea] PersistentTeamManager not found!");
            return;
        }

        // Kiểm tra team đã full HP chưa
        if (PersistentTeamManager.Instance.GetTotalCurrentHP() >= PersistentTeamManager.Instance.GetTotalMaxHP())
        {
            Debug.Log("[RestArea] Team already at full HP!");
            return;
        }

        // Kiểm tra coins
        if (MenuManager.Instance != null)
        {
            if (MenuManager.Instance.SpendCoins(fullHealCost))
            {
                PersistentTeamManager.Instance.HealTeamToFull();
                Debug.Log($"[RestArea] Team fully healed! (-{fullHealCost} coins)");

                UpdateCoinUI();
                PersistentTeamManager.Instance.LogTeamStatus();
            }
            else
            {
                Debug.LogWarning("[RestArea] Not enough coins for full heal!");
            }
        }
        else
        {
            // Không có MenuManager thì heal free
            PersistentTeamManager.Instance.HealTeamToFull();
            Debug.Log("[RestArea] Team fully healed (FREE)!");
            PersistentTeamManager.Instance.LogTeamStatus();
        }
    }

    // ===== NEW: PARTIAL HEAL =====
    public void PartialHealTeam()
    {
        if (PersistentTeamManager.Instance == null)
        {
            Debug.LogError("[RestArea] PersistentTeamManager not found!");
            return;
        }

        // Kiểm tra team đã full HP chưa
        if (PersistentTeamManager.Instance.GetTotalCurrentHP() >= PersistentTeamManager.Instance.GetTotalMaxHP())
        {
            Debug.Log("[RestArea] Team already at full HP!");
            return;
        }

        // Kiểm tra coins
        if (MenuManager.Instance != null)
        {
            if (MenuManager.Instance.SpendCoins(partialHealCost))
            {
                PersistentTeamManager.Instance.HealTeam(partialHealAmount);
                Debug.Log($"[RestArea] Team healed for {partialHealAmount} HP! (-{partialHealCost} coins)");

                UpdateCoinUI();
                PersistentTeamManager.Instance.LogTeamStatus();
            }
            else
            {
                Debug.LogWarning("[RestArea] Not enough coins for partial heal!");
            }
        }
        else
        {
            // Không có MenuManager thì heal free
            PersistentTeamManager.Instance.HealTeam(partialHealAmount);
            Debug.Log($"[RestArea] Team healed for {partialHealAmount} HP (FREE)!");
            PersistentTeamManager.Instance.LogTeamStatus();
        }
    }

    public void ExitToCasino()
    {
        if (shopManager != null)
        {
            shopManager.ResetShop();
        }

        SceneManager.LoadScene("Menu");
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
        if (coinText != null)
        {
            coinText.text = newCoin.ToString();
        }
    }
}