using UnityEngine;

[System.Serializable]
public class CharacterLevelData
{
    [Header("Level Settings")]
    public int currentLevel = 10;
    public const int MIN_LEVEL = 10;
    public const int MAX_LEVEL = 60;

    [Header("Stats Per Level")]
    public int hpPerLevel = 10;
    public int atkPerLevel = 3;
    public int defPerLevel = 1;

    [Header("Special Bonus Every 10 Levels")]
    public SpecialStatType specialStatType = SpecialStatType.Crit;
    [Range(1, 50)]
    public int specialStatBonus = 5;

    [Header("Level Up Cost")]
    public int baseExpCost = 100;
    public int baseGoldCost = 50;
    public float costMultiplierPerLevel = 1.1f;

    public int GetExpCostForNextLevel()
    {
        return Mathf.RoundToInt(baseExpCost * Mathf.Pow(costMultiplierPerLevel, currentLevel - MIN_LEVEL));
    }

    public int GetGoldCostForNextLevel()
    {
        return Mathf.RoundToInt(baseGoldCost * Mathf.Pow(costMultiplierPerLevel, currentLevel - MIN_LEVEL));
    }

    public bool CanLevelUp()
    {
        return currentLevel < MAX_LEVEL;
    }

    public bool IsSpecialLevel(int level)
    {
        return level > MIN_LEVEL && level % 10 == 0;
    }

    public void CalculateTotalStatsBonus(out int totalHP, out int totalATK, out int totalDEF, out int specialBonus)
    {
        int levelsGained = currentLevel - MIN_LEVEL;
        totalHP = levelsGained * hpPerLevel;
        totalATK = levelsGained * atkPerLevel;
        totalDEF = levelsGained * defPerLevel;

        int specialLevelCount = 0;
        for (int level = MIN_LEVEL + 10; level <= currentLevel; level += 10)
        {
            specialLevelCount++;
        }
        specialBonus = specialLevelCount * specialStatBonus;
    }
}

public enum SpecialStatType
{
    HP,
    ATK,
    DEF,
    Crit,
    CritDam
}
