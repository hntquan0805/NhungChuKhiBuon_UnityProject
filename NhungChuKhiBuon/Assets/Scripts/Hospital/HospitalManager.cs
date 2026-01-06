using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HospitalManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text coinText;
    public UnityEngine.UI.Button healButton;
    public UnityEngine.UI.Button reviveButton;

    [Header("Display")]
    public RestAreaTeamDisplay teamDisplay;

    [Header("Costs")]
    public int healAmount = 100;
    public int healCost = 10;
    public int reviveCost = 1000;

    private void Start()
    {
        UpdateCoinUI();

        healButton.onClick.AddListener(HealLowestHero);
        reviveButton.onClick.AddListener(ReviveHero);

        PersistentTeamManager.Instance?.LogTeamStatus();
    }

    // ===== HEAL HERO (GIỐNG REST AREA) =====
    public void HealLowestHero()
    {
        var team = PersistentTeamManager.Instance;
        if (team == null)
        {
            Debug.LogError("PersistentTeamManager is NULL!");
            return;
        }

        var hero = GetLowestHPHero(includeDead: false);
        if (hero == null)
        {
            Debug.Log("[Hospital] No hero found to heal");
            return;
        }

        // Kiểm tra trước khi trừ tiền
        Debug.Log($"[Hospital] Target: {hero.heroName}, HP Before: {hero.currentHP}/{hero.maxHP}");

        if (!SpendCoins(healCost))
        {
            Debug.LogWarning("[Hospital] Not enough coins!");
            return;
        }

        // THỰC HIỆN CỘNG MÁU
        int oldHP = hero.currentHP;
        hero.currentHP = Mathf.Min(hero.currentHP + healAmount, hero.maxHP);

        Debug.Log($"[Hospital] HP After Calc: {hero.currentHP}. (Gained: {hero.currentHP - oldHP})");

        // REFRESH HIỂN THỊ
        try
        {
            Refresh();
            Debug.Log("[Hospital] Refresh UI Successful");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Hospital] Refresh UI Failed: {e.Message}");
        }
    }

    // ===== REVIVE HERO CHẾT =====
    public void ReviveHero()
    {
        var team = PersistentTeamManager.Instance;
        if (team == null) return;

        var deadHero = GetLowestHPHero(includeDead: true, onlyDead: true);
        if (deadHero == null)
        {
            Debug.Log("[Hospital] No dead hero to revive");
            return;
        }

        if (!SpendCoins(reviveCost)) return;

        deadHero.currentHP = 1; // 🔥 HP = 1 khi hồi sinh
        Refresh();

        Debug.Log($"[Hospital] Revived {deadHero.heroName}");
    }

    // ===== HELPERS =====
    private PersistentTeamManager.HeroRuntimeData GetLowestHPHero(
        bool includeDead,
        bool onlyDead = false)
    {
        PersistentTeamManager.HeroRuntimeData result = null;

        foreach (var hero in PersistentTeamManager.Instance.teamData)
        {
            if (onlyDead && hero.currentHP > 0) continue;
            if (!includeDead && hero.currentHP <= 0) continue;
            if (!onlyDead && hero.currentHP >= hero.maxHP) continue;

            if (result == null || hero.currentHP < result.currentHP)
            {
                result = hero;
            }
        }

        return result;
    }

    private bool SpendCoins(int amount)
    {
        if (MenuManager.Instance == null) return true;

        if (!MenuManager.Instance.SpendCoins(amount))
        {
            Debug.LogWarning("[Hospital] Not enough coins");
            return false;
        }

        UpdateCoinUI();
        return true;
    }

    private void Refresh()
    {
        // Đảm bảo dữ liệu UI coin được cập nhật
        UpdateCoinUI();

        // Ép Display cập nhật lại các Hero đang đứng trong scene
        if (teamDisplay != null)
        {
            teamDisplay.RefreshDisplay();
        }

        // Log để kiểm tra xem dữ liệu thực tế đã tăng chưa
        PersistentTeamManager.Instance?.LogTeamStatus();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null && MenuManager.Instance != null)
        {
            coinText.text = MenuManager.Instance.PlayerCoins.ToString();
        }
    }

    public void ExitHospital()
    {
        SceneManager.LoadScene("Menu");
    }
}
