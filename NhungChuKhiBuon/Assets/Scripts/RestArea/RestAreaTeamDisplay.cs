using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawn team heroes trong Rest Area và kết nối với PlayerTeam
/// Team có tối đa 3 hero, spawn theo SLOT đặt sẵn trong scene
/// </summary>
public class RestAreaTeamDisplay : MonoBehaviour
{
    [Header("References")]
    public PlayerTeam playerTeam;
    public TeamHealthDisplay teamHealthDisplay;

    [Header("Hero Spawn Slots (Max 3)")]
    public Transform heroSpawnParent; // Parent chứa Slot_0, Slot_1, Slot_2
    public List<Transform> heroSlots = new List<Transform>();

    [Header("Individual Health Bar Settings")]
    public GameObject healthBarPrefab;
    public Transform healthBarParent;
    public Vector3 healthBarOffset = new Vector3(0f, 2f, 0f);

    private readonly List<GameObject> spawnedHeroes = new();
    private readonly List<GameObject> spawnedHealthBars = new();

    private void Awake()
    {
        CacheHeroSlots();
    }

    private void Start()
    {
        SpawnTeam();
    }

    /// <summary>
    /// Cache slot positions từ HeroSpawnContainer
    /// </summary>
    private void CacheHeroSlots()
    {
        heroSlots.Clear();

        if (heroSpawnParent == null)
        {
            Debug.LogError("[RestAreaDisplay] HeroSpawnParent not assigned!");
            return;
        }

        foreach (Transform child in heroSpawnParent)
        {
            heroSlots.Add(child);
        }

        if (heroSlots.Count == 0)
        {
            Debug.LogError("[RestAreaDisplay] No hero slots found!");
        }
    }

    /// <summary>
    /// Spawn heroes từ PersistentTeamManager vào các SLOT
    /// </summary>
    private void SpawnTeam()
    {
        if (PersistentTeamManager.Instance == null || playerTeam == null)
        {
            Debug.LogError("[RestAreaDisplay] Missing references!");
            return;
        }

        ClearSpawnedObjects();

        var teamData = PersistentTeamManager.Instance.teamData;
        int heroCount = Mathf.Min(teamData.Count, heroSlots.Count);

        for (int i = 0; i < heroCount; i++)
        {
            var heroData = teamData[i];
            Transform slot = heroSlots[i];

            if (heroData.heroPrefab == null || slot == null)
                continue;

            GameObject heroObj = Instantiate(
                heroData.heroPrefab,
                slot.position,
                slot.rotation,
                playerTeam.transform
            );

            // Lock position (tránh bị physics kéo lệch)
            heroObj.transform.position = slot.position;
            heroObj.transform.rotation = slot.rotation;

            PlayerCharacter pc = heroObj.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                pc.SetHP(heroData.currentHP, heroData.maxHP);
                DisableCombatComponents(heroObj);
            }

            spawnedHeroes.Add(heroObj);

            // Spawn individual health bar
            if (healthBarPrefab != null && healthBarParent != null)
            {
                GameObject barObj = Instantiate(healthBarPrefab, healthBarParent);
                spawnedHealthBars.Add(barObj);
                SetupHealthBar(barObj, heroData, heroObj.transform);
            }

            Debug.Log($"[RestAreaDisplay] Spawned {heroData.heroName} at Slot_{i}");
        }

        playerTeam.RefreshPlayers();
    }

    private void DisableCombatComponents(GameObject hero)
    {
        Collider2D col = hero.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = hero.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;
    }

    private void SetupHealthBar(
        GameObject healthBarObj,
        PersistentTeamManager.HeroRuntimeData heroData,
        Transform heroTransform)
    {
        var fillImage = healthBarObj.transform.Find("Fill")
            ?.GetComponent<UnityEngine.UI.Image>();

        var healthText = healthBarObj.transform.Find("HealthText")
            ?.GetComponent<TMPro.TextMeshProUGUI>();

        var tracker = healthBarObj.AddComponent<RestAreaHealthBarTracker>();
        tracker.heroData = heroData;
        tracker.fillImage = fillImage;
        tracker.healthText = healthText;
        tracker.heroTransform = heroTransform;
        tracker.offset = healthBarOffset;

        tracker.UpdateBar();
    }

    public void RefreshDisplay()
    {
        for (int i = 0; i < spawnedHeroes.Count; i++)
        {
            var heroObj = spawnedHeroes[i];
            var heroData = PersistentTeamManager.Instance.teamData[i];

            PlayerCharacter pc = heroObj.GetComponent<PlayerCharacter>();
            if (pc != null)
            {
                pc.SetHP(heroData.currentHP, heroData.maxHP);
            }
        }

        foreach (var bar in spawnedHealthBars)
        {
            bar?.GetComponent<RestAreaHealthBarTracker>()?.UpdateBar();
        }

        teamHealthDisplay?.RefreshImmediate();
    }

    private void ClearSpawnedObjects()
    {
        foreach (var h in spawnedHeroes) Destroy(h);
        foreach (var b in spawnedHealthBars) Destroy(b);
        spawnedHeroes.Clear();
        spawnedHealthBars.Clear();
    }

    private void OnDestroy()
    {
        ClearSpawnedObjects();
    }
}

/// <summary>
/// Theo dõi & update individual health bar
/// </summary>
public class RestAreaHealthBarTracker : MonoBehaviour
{
    public PersistentTeamManager.HeroRuntimeData heroData;
    public UnityEngine.UI.Image fillImage;
    public TMPro.TextMeshProUGUI healthText;
    public Transform heroTransform;
    public Vector3 offset;

    private void Update()
    {
        UpdateBar();
    }

    public void UpdateBar()
    {
        if (heroData == null || heroTransform == null) return;

        transform.position = Camera.main.WorldToScreenPoint(
            heroTransform.position + offset
        );

        if (fillImage != null)
        {
            float ratio = (float)heroData.currentHP / heroData.maxHP;
            fillImage.transform.localScale = new Vector3(
                Mathf.Clamp01(ratio), 1f, 1f
            );
        }

        if (healthText != null)
        {
            healthText.text = $"{heroData.currentHP}/{heroData.maxHP}";
        }
    }
}
