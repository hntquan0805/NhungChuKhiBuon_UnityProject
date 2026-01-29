using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Quản lý trang bị của từng character
/// Lưu trữ trong PlayerPrefs
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    private static EquipmentManager instance;
    public static EquipmentManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("EquipmentManager");
                instance = go.AddComponent<EquipmentManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // Key format: "Equipment_{characterName}_{slotType}"
    private const string EQUIPMENT_KEY_FORMAT = "Equipment_{0}_{1}";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Lắp trang bị cho character
    /// </summary>
    public bool EquipItem(string characterName, ItemInstance item)
    {
        Debug.Log($"[EquipManager] EquipItem char={characterName}, item={item?.baseData?.itemName}");
        if (item == null || item.baseData == null)
        {
            Debug.LogError("[EquipmentManager] Item is null!");
            return false;
        }

        // Kiểm tra xem slot này đã có trang bị chưa
        ItemType slotType = item.baseData.type;
        ItemInstance currentItem = GetEquippedItem(characterName, slotType);

        if (currentItem != null)
        {
            // Trả lại item cũ vào inventory
            InventoryManager.Instance.AddItem(currentItem);
        }

        // Xóa item khỏi inventory
        InventoryManager.Instance.RemoveItem(item);

        // Lưu trang bị mới
        SaveEquipment(characterName, slotType, item);

        Debug.Log($"[EquipmentManager] Equipped {item.baseData.itemName} to {characterName}");
        return true;
    }

    /// <summary>
    /// Tháo trang bị
    /// </summary>
    public bool UnequipItem(string characterName, ItemType slotType)
    {
        ItemInstance item = GetEquippedItem(characterName, slotType);

        if (item == null)
        {
            Debug.LogWarning("[EquipmentManager] No item equipped in this slot!");
            return false;
        }

        // Trả lại vào inventory
        InventoryManager.Instance.AddItem(item);

        // Xóa khỏi equipment
        ClearEquipment(characterName, slotType);

        Debug.Log($"[EquipmentManager] Unequipped {item.baseData.itemName} from {characterName}");
        return true;
    }

    /// <summary>
    /// Lấy trang bị hiện tại ở slot
    /// </summary>
    public ItemInstance GetEquippedItem(string characterName, ItemType slotType)
    {
        string key = string.Format(EQUIPMENT_KEY_FORMAT, characterName, slotType);
        string json = PlayerPrefs.GetString(key, "");

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<ItemInstance>(json);
    }

    /// <summary>
    /// Lấy tất cả trang bị của character
    /// </summary>
    public Dictionary<ItemType, ItemInstance> GetAllEquipment(string characterName)
    {
        Dictionary<ItemType, ItemInstance> equipment = new Dictionary<ItemType, ItemInstance>();

        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            ItemInstance item = GetEquippedItem(characterName, type);
            if (item != null)
            {
                equipment[type] = item;
            }
        }

        return equipment;
    }

    /// <summary>
    /// Tính toán bonus stats từ trang bị
    /// </summary>
    public EquipmentStats CalculateEquipmentStats(string characterName, PlayerStats baseStats)
    {
        EquipmentStats result = new EquipmentStats();
        Dictionary<ItemType, ItemInstance> equipment = GetAllEquipment(characterName);

        // Thu thập tất cả stats từ trang bị
        Dictionary<StatType, float> flatStats = new Dictionary<StatType, float>();
        Dictionary<StatType, float> percentStats = new Dictionary<StatType, float>();

        foreach (var item in equipment.Values)
        {
            // Main stat
            AddStat(item.mainStat, flatStats, percentStats);

            // Sub stats
            foreach (var subStat in item.subStats)
            {
                AddStat(subStat, flatStats, percentStats);
            }
        }

        // Kiểm tra set bonus
        float setBonus = CalculateSetBonus(equipment);

        // Tính toán stats cuối cùng
        // Attack
        float attackFlat = GetStatValue(flatStats, StatType.AttackFlat);
        float attackPercent = GetStatValue(percentStats, StatType.AttackPercent) + setBonus;
        result.bonusAttack = Mathf.RoundToInt(attackFlat + baseStats.atk * attackPercent / 100f);

        // Defense
        float defenseFlat = GetStatValue(flatStats, StatType.DefenseFlat);
        float defensePercent = GetStatValue(percentStats, StatType.DefensePercent) + setBonus;
        result.bonusDefense = Mathf.RoundToInt(defenseFlat + baseStats.def * defensePercent / 100f);

        // Health
        float healthFlat = GetStatValue(flatStats, StatType.HealthFlat);
        float healthPercent = GetStatValue(percentStats, StatType.HealthPercent) + setBonus;
        result.bonusHealth = Mathf.RoundToInt(healthFlat + baseStats.maxHP * healthPercent / 100f);

        // Crit Rate
        result.bonusCritRate = Mathf.RoundToInt(GetStatValue(flatStats, StatType.CritRate));

        // Accuracy & Resistance (để sau nếu cần)
        result.bonusAccuracy = Mathf.RoundToInt(GetStatValue(flatStats, StatType.Accuracy));
        result.bonusResistance = Mathf.RoundToInt(GetStatValue(flatStats, StatType.Resistance));

        return result;
    }

    /// <summary>
    /// Tính toán set bonus
    /// </summary>
    private float CalculateSetBonus(Dictionary<ItemType, ItemInstance> equipment)
    {
        // Đếm số món của mỗi set
        Dictionary<ItemSet, int> setCount = new Dictionary<ItemSet, int>();

        foreach (var item in equipment.Values)
        {
            ItemSet set = item.baseData.set;
            if (!setCount.ContainsKey(set))
                setCount[set] = 0;

            setCount[set]++;
        }

        float bonus = 0f;

        foreach (var kvp in setCount)
        {
            ItemSet set = kvp.Key;
            int count = kvp.Value;

            // Health, Defense, Attack set: cần 2 món -> +20%
            if ((set == ItemSet.Health || set == ItemSet.Defense || set == ItemSet.Attack) && count >= 2)
            {
                bonus += 30f;
            }
            // Poison, Scoot set: cần 3 món (chưa làm hiệu ứng)
            else if ((set == ItemSet.Poison || set == ItemSet.Scoot) && count >= 3)
            {
                // TODO: Implement special effects
                Debug.Log($"[EquipmentManager] {set} set bonus activated (not implemented yet)");
            }
        }

        return bonus;
    }

    /// <summary>
    /// Helper: Thêm stat vào dictionary
    /// </summary>
    private void AddStat(ItemStat stat, Dictionary<StatType, float> flatStats, Dictionary<StatType, float> percentStats)
    {
        if (IsPercentStat(stat.statType))
        {
            if (!percentStats.ContainsKey(stat.statType))
                percentStats[stat.statType] = 0;
            percentStats[stat.statType] += stat.value;
        }
        else
        {
            if (!flatStats.ContainsKey(stat.statType))
                flatStats[stat.statType] = 0;
            flatStats[stat.statType] += stat.value;
        }
    }

    /// <summary>
    /// Helper: Kiểm tra stat có phải % không
    /// </summary>
    private bool IsPercentStat(StatType type)
    {
        return type == StatType.AttackPercent ||
               type == StatType.DefensePercent ||
               type == StatType.HealthPercent;
    }

    /// <summary>
    /// Helper: Lấy giá trị stat
    /// </summary>
    private float GetStatValue(Dictionary<StatType, float> stats, StatType type)
    {
        return stats.ContainsKey(type) ? stats[type] : 0f;
    }

    /// <summary>
    /// Lưu trang bị vào PlayerPrefs
    /// </summary>
    private void SaveEquipment(string characterName, ItemType slotType, ItemInstance item)
    {
        string key = string.Format(EQUIPMENT_KEY_FORMAT, characterName, slotType);
        string json = JsonUtility.ToJson(item);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Xóa trang bị
    /// </summary>
    private void ClearEquipment(string characterName, ItemType slotType)
    {
        string key = string.Format(EQUIPMENT_KEY_FORMAT, characterName, slotType);
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Reset tất cả trang bị của character
    /// </summary>
    public void ClearAllEquipment(string characterName)
    {
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            UnequipItem(characterName, type);
        }
    }
}

/// <summary>
/// Struct chứa bonus stats từ trang bị
/// </summary>
[System.Serializable]
public struct EquipmentStats
{
    public int bonusAttack;
    public int bonusDefense;
    public int bonusHealth;
    public int bonusCritRate;
    public int bonusAccuracy;
    public int bonusResistance;
}