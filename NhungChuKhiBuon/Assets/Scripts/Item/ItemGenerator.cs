using System.Collections.Generic;
using UnityEngine;

public static class ItemGenerator
{
    // ===== MAIN STAT VALUES BY TIER =====
    // [Basic, Mid, High]
    static readonly Dictionary<StatType, int[]> MainStatValues = new Dictionary<StatType, int[]>
    {
        { StatType.AttackFlat,      new int[] { 20, 30, 50 } },
        { StatType.AttackPercent,   new int[] { 10, 20, 40 } },
        { StatType.DefenseFlat,     new int[] { 30, 40, 60 } },
        { StatType.DefensePercent,  new int[] { 10, 20, 40 } },
        { StatType.HealthFlat,      new int[] { 100, 300, 500 } },
        { StatType.HealthPercent,   new int[] { 20, 30, 50 } },
        { StatType.CritRate,        new int[] { 10, 20, 30 } },
        { StatType.Accuracy,        new int[] { 10, 20, 30 } },
        { StatType.Resistance,      new int[] { 10, 20, 30 } }
    };

    // ===== SUB STAT POOL =====
    static readonly StatType[] SubStatPool =
    {
        StatType.CritRate,
        StatType.Accuracy,
        StatType.Resistance,
        StatType.AttackFlat,
        StatType.AttackPercent,
        StatType.DefenseFlat,
        StatType.DefensePercent,
        StatType.HealthFlat,
        StatType.HealthPercent
    };

    // ===== PUBLIC API =====
    public static ItemInstance CreateItem(ItemData data)
    {
        ItemInstance item = new ItemInstance();
        item.baseData = data;
        item.mainStat = GenerateMainStat(data.type, data.tier);

        int subCount = GetSubStatCount(data.tier);
        item.subStats = GenerateSubStats(subCount, item.mainStat.statType);

        return item;
    }

    // ===== MAIN STAT =====
    static ItemStat GenerateMainStat(ItemType type, ItemTier tier)
    {
        StatType mainStatType;

        // Xác định StatType dựa trên ItemType
        switch (type)
        {
            case ItemType.Weapon:
                mainStatType = StatType.AttackFlat;
                break;

            case ItemType.Armor:
                mainStatType = StatType.DefenseFlat;
                break;

            case ItemType.Accessory:
                // Accessory có thể có nhiều loại main stat
                StatType[] accessoryMainStats =
                {
                    StatType.CritRate,
                    StatType.Accuracy,
                    StatType.Resistance,
                    StatType.AttackPercent,
                    StatType.DefensePercent,
                    StatType.HealthPercent
                };
                mainStatType = accessoryMainStats[Random.Range(0, accessoryMainStats.Length)];
                break;

            default:
                mainStatType = StatType.AttackFlat;
                break;
        }

        // Lấy giá trị theo tier
        int value = GetMainStatValue(mainStatType, tier);

        return new ItemStat
        {
            statType = mainStatType,
            value = value
        };
    }

    // ===== GET MAIN STAT VALUE BY TIER =====
    static int GetMainStatValue(StatType statType, ItemTier tier)
    {
        if (!MainStatValues.ContainsKey(statType))
        {
            Debug.LogWarning($"StatType {statType} không có trong MainStatValues!");
            return 10;
        }

        int tierIndex = (int)tier; // Basic=0, Mid=1, High=2
        return MainStatValues[statType][tierIndex];
    }

    // ===== GET SUB STAT VALUE (1/10 của Mid tier) =====
    static int GetSubStatValue(StatType statType)
    {
        if (!MainStatValues.ContainsKey(statType))
        {
            Debug.LogWarning($"StatType {statType} không có trong MainStatValues!");
            return 1;
        }

        // Lấy giá trị Mid tier (index = 1) rồi chia 10
        int midValue = MainStatValues[statType][1];
        return Mathf.Max(1, midValue / 10); // Tối thiểu là 1
    }

    // ===== SUB STAT COUNT =====
    static int GetSubStatCount(ItemTier tier)
    {
        switch (tier)
        {
            case ItemTier.Basic: return 2;
            case ItemTier.Mid: return 3;
            case ItemTier.High: return 4;
            default: return 2;
        }
    }

    // ===== SUB STAT GENERATION =====
    static List<ItemStat> GenerateSubStats(int count, StatType mainStatType)
    {
        List<ItemStat> result = new List<ItemStat>();
        List<StatType> pool = new List<StatType>(SubStatPool);

        // KHÔNG loại bỏ main stat, cho phép trùng với main stat

        for (int i = 0; i < count; i++)
        {
            if (pool.Count == 0) break;

            StatType stat = pool[Random.Range(0, pool.Count)];
            pool.Remove(stat); // Loại bỏ khỏi pool để không bị trùng lặp giữa các sub stat

            result.Add(new ItemStat
            {
                statType = stat,
                value = GetSubStatValue(stat)
            });
        }

        return result;
    }
}