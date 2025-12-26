using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý trạng thái team xuyên suốt game - HP, stats, shields, v.v.
/// Singleton với DontDestroyOnLoad
/// </summary>
public class PersistentTeamManager : MonoBehaviour
{
    public static PersistentTeamManager Instance;

    [System.Serializable]
    public class HeroRuntimeData
    {
        public string heroName;
        public GameObject heroPrefab;

        // Runtime stats
        public int currentHP;
        public int maxHP;
        public int currentShield;

        // Original stats từ prefab (để restore nếu cần)
        public PlayerStats originalStats;

        // Flag để track hero đã được khởi tạo chưa
        public bool isInitialized;

        public HeroRuntimeData(string name, GameObject prefab)
        {
            heroName = name;
            heroPrefab = prefab;
            isInitialized = false;
            currentShield = 0;
        }
    }

    [Header("Runtime Team Data")]
    public List<HeroRuntimeData> teamData = new List<HeroRuntimeData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Khởi tạo team data từ TeamDataManager (gọi sau khi chọn team)
    /// </summary>
    public void InitializeFromTeamSelection()
    {
        if (TeamDataManager.Instance == null)
        {
            Debug.LogError("[PersistentTeam] TeamDataManager not found!");
            return;
        }

        teamData.Clear();

        var selectedTeam = TeamDataManager.Instance.GetSelectedTeam();

        foreach (var hero in selectedTeam)
        {
            if (hero.heroPrefab == null)
            {
                Debug.LogWarning($"[PersistentTeam] Hero {hero.heroName} missing prefab!");
                continue;
            }

            HeroRuntimeData runtimeData = new HeroRuntimeData(hero.heroName, hero.heroPrefab);

            // Lấy stats từ prefab
            PlayerCharacter prefabChar = hero.heroPrefab.GetComponent<PlayerCharacter>();
            if (prefabChar != null)
            {
                runtimeData.maxHP = prefabChar.stats.maxHP;
                runtimeData.currentHP = prefabChar.stats.maxHP; // Bắt đầu với HP đầy
                runtimeData.originalStats = new PlayerStats(prefabChar.stats);
                runtimeData.isInitialized = true;
            }

            teamData.Add(runtimeData);
        }

        Debug.Log($"[PersistentTeam] Initialized {teamData.Count} heroes with full HP");
    }

    /// <summary>
    /// Lưu trạng thái hiện tại của team từ battle scene
    /// </summary>
    public void SaveTeamState(List<PlayerCharacter> players)
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogWarning("[PersistentTeam] No players to save!");
            return;
        }

        for (int i = 0; i < players.Count && i < teamData.Count; i++)
        {
            var player = players[i];
            var data = teamData[i];

            data.currentHP = player.GetCurrentHP();
            data.maxHP = player.GetMaxHP();
            data.currentShield = player.GetShieldAmount();

            Debug.Log($"[PersistentTeam] Saved {data.heroName}: HP={data.currentHP}/{data.maxHP}, Shield={data.currentShield}");
        }
    }

    /// <summary>
    /// Áp dụng trạng thái đã lưu lên team trong battle scene
    /// </summary>
    public void ApplyTeamState(List<PlayerCharacter> players)
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogWarning("[PersistentTeam] No players to apply state!");
            return;
        }

        for (int i = 0; i < players.Count && i < teamData.Count; i++)
        {
            var player = players[i];
            var data = teamData[i];

            // Set HP trực tiếp (bypass animation)
            player.SetHP(data.currentHP, data.maxHP);

            // Set shield nếu có
            if (data.currentShield > 0)
            {
                player.AddShieldSilent(data.currentShield);
            }

            Debug.Log($"[PersistentTeam] Applied state to {data.heroName}: HP={data.currentHP}/{data.maxHP}, Shield={data.currentShield}");
        }
    }

    /// <summary>
    /// Heal toàn bộ team về full HP (dùng cho Rest Area)
    /// </summary>
    public void HealTeamToFull()
    {
        foreach (var data in teamData)
        {
            data.currentHP = data.maxHP;
            data.currentShield = 0;
        }

        Debug.Log("[PersistentTeam] Team healed to full HP");
    }

    /// <summary>
    /// Heal team một lượng cụ thể
    /// </summary>
    public void HealTeam(int amount)
    {
        foreach (var data in teamData)
        {
            data.currentHP = Mathf.Min(data.currentHP + amount, data.maxHP);
        }

        Debug.Log($"[PersistentTeam] Team healed for {amount} HP");
    }

    /// <summary>
    /// Giảm HP toàn team (cho Arena debuff)
    /// </summary>
    public void ReduceTeamHP(float percent)
    {
        foreach (var data in teamData)
        {
            int damage = Mathf.RoundToInt(data.maxHP * percent);
            damage = Mathf.Min(damage, data.currentHP - 1); // Không chết
            data.currentHP -= damage;
        }

        Debug.Log($"[PersistentTeam] Team HP reduced by {percent * 100}%");
    }

    /// <summary>
    /// Lấy tổng HP hiện tại của team
    /// </summary>
    public int GetTotalCurrentHP()
    {
        int total = 0;
        foreach (var data in teamData)
        {
            total += data.currentHP;
        }
        return total;
    }

    /// <summary>
    /// Lấy tổng max HP của team
    /// </summary>
    public int GetTotalMaxHP()
    {
        int total = 0;
        foreach (var data in teamData)
        {
            total += data.maxHP;
        }
        return total;
    }

    /// <summary>
    /// Check team có còn sống không
    /// </summary>
    public bool IsTeamAlive()
    {
        foreach (var data in teamData)
        {
            if (data.currentHP > 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Clear tất cả data (khi chọn team mới)
    /// </summary>
    public void ClearTeamData()
    {
        teamData.Clear();
        Debug.Log("[PersistentTeam] Team data cleared");
    }

    /// <summary>
    /// Get hero data by index
    /// </summary>
    public HeroRuntimeData GetHeroData(int index)
    {
        if (index >= 0 && index < teamData.Count)
            return teamData[index];
        return null;
    }

    /// <summary>
    /// Log team status (debug)
    /// </summary>
    public void LogTeamStatus()
    {
        Debug.Log("=== TEAM STATUS ===");
        for (int i = 0; i < teamData.Count; i++)
        {
            var data = teamData[i];
            Debug.Log($"[{i}] {data.heroName}: {data.currentHP}/{data.maxHP} HP, Shield: {data.currentShield}");
        }
        Debug.Log($"Total: {GetTotalCurrentHP()}/{GetTotalMaxHP()} HP");
        Debug.Log("==================");
    }
}