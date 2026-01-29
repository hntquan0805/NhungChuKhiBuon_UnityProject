using UnityEngine;

/// <summary>
/// Apply equipment bonuses to PlayerCharacter stats
/// Attach this to PlayerCharacter prefabs or call during battle initialization
/// </summary>
public class EquipmentStatsApplier : MonoBehaviour
{
    private PlayerCharacter playerCharacter;
    private bool isApplied = false;

    private void Awake()
    {
        playerCharacter = GetComponent<PlayerCharacter>();
    }

    private void Start()
    {
        // Auto-apply equipment stats at start
        if (!isApplied)
        {
            ApplyEquipmentStats();
        }
    }

    /// <summary>
    /// Apply equipment bonuses to character stats
    /// Should be called after character is spawned in battle
    /// </summary>
    public void ApplyEquipmentStats()
    {
        if (playerCharacter == null)
        {
            Debug.LogWarning("[EquipmentStatsApplier] PlayerCharacter not found!");
            return;
        }

        if (isApplied)
        {
            Debug.LogWarning("[EquipmentStatsApplier] Equipment stats already applied!");
            return;
        }

        string charName = playerCharacter.stats.characterName;

        // Get base stats before equipment
        PlayerStats baseStats = playerCharacter.stats;
        int baseMaxHP = baseStats.maxHP;
        int baseAtk = baseStats.atk;
        int baseDef = baseStats.def;
        int baseCrit = baseStats.crit;

        // Calculate equipment bonuses
        EquipmentStats equipStats = EquipmentManager.Instance.CalculateEquipmentStats(charName, baseStats);

        // Apply bonuses to base stats
        // Note: We modify the base stats directly
        baseStats.baseMaxHP += equipStats.bonusHealth;
        baseStats.baseAtk += equipStats.bonusAttack;
        baseStats.baseDef += equipStats.bonusDefense;
        baseStats.baseCrit += equipStats.bonusCritRate;

        // Refresh current HP to match new max HP
        int currentHP = playerCharacter.GetCurrentHP();
        float hpPercent = (float)currentHP / baseMaxHP;
        int newCurrentHP = Mathf.RoundToInt((baseMaxHP + equipStats.bonusHealth) * hpPercent);
        playerCharacter.SetHP(newCurrentHP, baseMaxHP + equipStats.bonusHealth);

        isApplied = true;

        Debug.Log($"[EquipmentStatsApplier] Applied equipment to {charName}:");
        Debug.Log($"  HP: {baseMaxHP} → {baseMaxHP + equipStats.bonusHealth} (+{equipStats.bonusHealth})");
        Debug.Log($"  ATK: {baseAtk} → {baseAtk + equipStats.bonusAttack} (+{equipStats.bonusAttack})");
        Debug.Log($"  DEF: {baseDef} → {baseDef + equipStats.bonusDefense} (+{equipStats.bonusDefense})");
        Debug.Log($"  CRIT: {baseCrit}% → {baseCrit + equipStats.bonusCritRate}% (+{equipStats.bonusCritRate}%)");
    }

    /// <summary>
    /// Get equipment stats without applying them
    /// </summary>
    public EquipmentStats GetEquipmentStats()
    {
        if (playerCharacter == null)
            return new EquipmentStats();

        return EquipmentManager.Instance.CalculateEquipmentStats(
            playerCharacter.stats.characterName,
            playerCharacter.stats
        );
    }
}