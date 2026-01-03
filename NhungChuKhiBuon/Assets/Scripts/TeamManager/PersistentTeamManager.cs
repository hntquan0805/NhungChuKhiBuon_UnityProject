using UnityEngine;
using System.Collections.Generic;

// Quản lý trạng thái team xuyên suốt game (Singleton với DontDestroyOnLoad)
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

        // Original stats từ prefab (để restore nếu cần)
        public PlayerStats originalStats;

        // Flag để track hero đã được khởi tạo chưa
        public bool isInitialized;

        public HeroRuntimeData(string name, GameObject prefab)
        {
            heroName = name;
            heroPrefab = prefab;
            isInitialized = false;
        }
    }

    [Header("Runtime Team Data")]
    public List<HeroRuntimeData> teamData = new List<HeroRuntimeData>();

    [Header("Team Shield")]
    public int teamShield = 0; // 🔥 SHIELD CHUNG CỦA TEAM

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

    // Khởi tạo team data từ TeamDataManager
    public void InitializeFromTeamSelection()
    {
        if (TeamDataManager.Instance == null)
        {
            Debug.LogError("[PersistentTeam] TeamDataManager not found!");
            return;
        }

        teamData.Clear();
        teamShield = 0; // Reset shield về 0 khi khởi tạo team mới

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
    }

    // Lưu trạng thái team từ battle scene
    public void SaveTeamState(List<PlayerCharacter> players)
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogWarning("[PersistentTeam] No players to save!");
            return;
        }

        // Lưu HP của từng player
        for (int i = 0; i < players.Count && i < teamData.Count; i++)
        {
            var player = players[i];
            var data = teamData[i];

            data.currentHP = player.GetCurrentHP();
            data.maxHP = player.GetMaxHP();

        }

        // 🔥 LƯU TEAM SHIELD
        PlayerTeam team = players[0].GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            teamShield = team.GetTeamShield();
        }
    }

    // Áp dụng trạng thái đã lưu lên team
    public void ApplyTeamState(List<PlayerCharacter> players)
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogWarning("[PersistentTeam] No players to apply state!");
            return;
        }

        // Apply HP cho từng player
        for (int i = 0; i < players.Count && i < teamData.Count; i++)
        {
            var player = players[i];
            var data = teamData[i];

            // Set HP trực tiếp (bypass animation)
            player.SetHP(data.currentHP, data.maxHP);
        }

        // 🔥 APPLY TEAM SHIELD
        PlayerTeam team = players[0].GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            // Clear shield cũ trước
            team.ClearShield();

            // Set shield đã lưu
            if (teamShield > 0)
            {
                team.AddShield(teamShield);
            }
        }
    }

    // Heal toàn bộ team về full HP
    public void HealTeamToFull()
    {
        foreach (var data in teamData)
        {
            data.currentHP = data.maxHP;
        }

        teamShield = 0; // Clear shield khi heal full
    }

    // Heal team một lượng cụ thể
    public void HealTeam(int amount)
    {
        foreach (var data in teamData)
        {
            data.currentHP = Mathf.Min(data.currentHP + amount, data.maxHP);
        }
    }

    // Giảm HP toàn team (cho Arena debuff)
    public void ReduceTeamHP(float percent)
    {
        foreach (var data in teamData)
        {
            int damage = Mathf.RoundToInt(data.maxHP * percent);
            damage = Mathf.Min(damage, data.currentHP - 1); // Không chết
            data.currentHP -= damage;
        }

    }

    // Thêm shield cho team
    public void AddTeamShield(int amount)
    {
        teamShield += amount;
        teamShield = Mathf.Max(teamShield, 0);
    }

    // Clear team shield
    public void ClearTeamShield()
    {
        teamShield = 0;

    }

    // Lấy tổng HP hiện tại của team
    public int GetTotalCurrentHP()
    {
        int total = 0;
        foreach (var data in teamData)
        {
            total += data.currentHP;
        }
        return total;
    }

    // Lấy tổng max HP của team
    public int GetTotalMaxHP()
    {
        int total = 0;
        foreach (var data in teamData)
        {
            total += data.maxHP;
        }
        return total;
    }

    // Lấy team shield hiện tại
    public int GetTeamShield()
    {
        return teamShield;
    }

    // Check team có còn sống không
    public bool IsTeamAlive()
    {
        foreach (var data in teamData)
        {
            if (data.currentHP > 0)
                return true;
        }
        return false;
    }

    // Clear tất cả data
    public void ClearTeamData()
    {
        teamData.Clear();
        teamShield = 0;
    }

    // Get hero data by index
    public HeroRuntimeData GetHeroData(int index)
    {
        if (index >= 0 && index < teamData.Count)
            return teamData[index];
        return null;
    }

    // Log team status (debug)
    public void LogTeamStatus()
    {
        // Debugging method - intentionally empty for production
    }
}