using UnityEngine;

public class BattleSceneInitializer : MonoBehaviour
{
    [Header("References")]
    public Transform playerTeamParent;

    [Header("Spawn Settings")]
    public Vector3 spawnStartPosition = new Vector3(-3f, 0f, 0f);
    public float spawnSpacing = 2f;

    [Header("Scene Type")]
    public bool isArenaScene = false;
    [Range(0f, 1f)]
    public float arenaHPReduction = 0.3f;

    private PlayerTeam playerTeam;

    [Header("Hero Slots (Editor Assigned)")]
    public Transform[] heroSlots;

    private void Start()
    {
        if (PersistentTeamManager.Instance == null)
        {
            Debug.LogError("[BattleInit] PersistentTeamManager not found!");
            return;
        }

        if (PersistentTeamManager.Instance.teamData.Count == 0)
        {
            PersistentTeamManager.Instance.InitializeFromTeamSelection();
        }

        SpawnPlayerTeam();
        ApplyPersistentState(); // ✅ GỌI NGAY – KHÔNG DELAY
    }

    private void SpawnPlayerTeam()
    {
        if (playerTeamParent == null || heroSlots == null || heroSlots.Length == 0)
        {
            Debug.LogError("[BattleInit] Missing PlayerTeamParent or HeroSlots!");
            return;
        }

        playerTeam = playerTeamParent.GetComponent<PlayerTeam>();
        if (playerTeam == null)
        {
            Debug.LogError("[BattleInit] PlayerTeam component not found!");
            return;
        }

        // Xoá hero cũ
        foreach (Transform child in playerTeamParent)
            Destroy(child.gameObject);

        var teamData = PersistentTeamManager.Instance.teamData;

        int spawnCount = Mathf.Min(teamData.Count, heroSlots.Length);

        Debug.Log($"[BattleInit] Spawning {spawnCount} heroes into slots");

        for (int i = 0; i < spawnCount; i++)
        {
            var heroData = teamData[i];
            if (heroData.heroPrefab == null) continue;

            Transform slot = heroSlots[i];

            GameObject heroObj = Instantiate(
                heroData.heroPrefab,
                slot.position,
                slot.rotation,
                playerTeamParent
            );

            heroObj.name = heroData.heroName;
        }

        playerTeam.RefreshPlayers();

        if (BattleManager.Instance != null)
            BattleManager.Instance.playerTeam = playerTeam;
    }


    private void ApplyPersistentState()
    {
        if (playerTeam == null || playerTeam.players.Count == 0)
        {
            Debug.LogError("[BattleInit] PlayerTeam not ready!");
            return;
        }

        PersistentTeamManager.Instance.ApplyTeamState(playerTeam.players);

        if (isArenaScene)
        {
            PersistentTeamManager.Instance.ReduceTeamHP(arenaHPReduction);
            PersistentTeamManager.Instance.ApplyTeamState(playerTeam.players);

            Debug.Log($"[BattleInit] Arena debuff applied: -{arenaHPReduction * 100}% HP");
        }

        PersistentTeamManager.Instance.LogTeamStatus();
    }

    private void OnDestroy()
    {
        SaveTeamState();
    }

    private void OnApplicationQuit()
    {
        SaveTeamState();
    }

    public void SaveTeamState()
    {
        if (playerTeam != null && playerTeam.players.Count > 0)
        {
            PersistentTeamManager.Instance.SaveTeamState(playerTeam.players);
            Debug.Log("[BattleInit] Team state saved");
        }
    }
}
