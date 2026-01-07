using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RestAreaManager : MonoBehaviour
{
    public ShopManager shopManager;
    public TMP_Text coinText;

    [Header("Team Display")]
    public RestAreaTeamDisplay teamDisplay;

    [Header("Heal Settings")]
    public int healAmount = 100;
    public int healCost = 10;

    [Header("UI Button")]
    public UnityEngine.UI.Button healButton;

    private void Start()
    {
        UpdateCoinUI();

        if (healButton != null)
        {
            healButton.onClick.AddListener(HealLowestHero);
        }

        if (PersistentTeamManager.Instance != null)
        {
            PersistentTeamManager.Instance.LogTeamStatus();
        }
    }

    // ===== HEAL HERO CÓ HP THẤP NHẤT =====
    public void HealLowestHero()
    {
        var team = PersistentTeamManager.Instance;
        if (team == null)
        {
            Debug.LogError("[RestArea] PersistentTeamManager not found!");
            return;
        }

        // Tìm hero có HP thấp nhất nhưng chưa full
        PersistentTeamManager.HeroRuntimeData lowestHero = null;

        foreach (var hero in team.teamData)
        {
            if (hero.currentHP > 0 && hero.currentHP < hero.maxHP)
            {
                if (lowestHero == null || hero.currentHP < lowestHero.currentHP)
                {
                    lowestHero = hero;
                }
            }
        }

        if (lowestHero == null)
        {
            Debug.Log("[RestArea] All heroes are already at full HP!");
            return;
        }

        // Check coins
        if (MenuManager.Instance != null)
        {
            if (!MenuManager.Instance.SpendCoins(healCost))
            {
                Debug.LogWarning("[RestArea] Not enough coins!");
                return;
            }
        }

        // Heal hero
        int beforeHP = lowestHero.currentHP;
        lowestHero.currentHP = Mathf.Min(
            lowestHero.currentHP + healAmount,
            lowestHero.maxHP
        );

        Debug.Log(
            $"[RestArea] Healed {lowestHero.heroName}: " +
            $"{beforeHP} → {lowestHero.currentHP} (-{healCost} coins)"
        );

        UpdateCoinUI();
        RefreshTeamDisplay();
        team.LogTeamStatus();
    }

    private void RefreshTeamDisplay()
    {
        if (teamDisplay != null)
        {
            teamDisplay.RefreshDisplay();
        }
    }

    public void ExitToCasino()
    {
        if (shopManager != null)
        {
            shopManager.ResetShop();
        }

        SceneManager.LoadScene("Map");
    }

    private void OnEnable()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.OnCoinsChanged += UpdateCoinUI;
    }

    private void OnDisable()
    {
        if (MenuManager.Instance != null)
            MenuManager.Instance.OnCoinsChanged -= UpdateCoinUI;
    }

    public void UpdateCoinUI()
    {
        if (coinText != null && MenuManager.Instance != null)
        {
            coinText.text = MenuManager.Instance.PlayerCoins.ToString();
        }
    }

    public void UpdateCoinUI(int newCoin)
    {
        if (coinText != null)
        {
            coinText.text = newCoin.ToString();
        }
    }
}
