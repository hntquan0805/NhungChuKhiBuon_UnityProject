using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [Header("Base Stats (Level 10)")]
    public int baseMaxHP = 100;
    public int baseAtk = 50;        // Sát thương cơ bản
    public int baseDef = 10;        // Phòng thủ (%)

    [Header("Character Info")]
    public Sprite avatarSprite; // Avatar nhỏ hiển thị trong khu vực chọn hero
    public Sprite characterIcon; // Hình full body hiển thị trong slot đã chọn
    public string characterName = "Character"; // Tên nhân vật

    [Header("Critical Stats")]
    [Range(0, 100)]
    public int baseCrit = 15;       // Tỉ lệ chí mạng (%)
    [Range(100, 300)]
    public int baseCritDam = 150;   // Sát thương chí mạng (%)

    [Header("Level System")]
    public CharacterLevelData levelData = new CharacterLevelData();

    [Header("Class")]
    public CharacterClass characterClass = CharacterClass.Knight;

    // Tính toán stats cuối cùng với level bonus
    public int maxHP
    {
        get
        {
            int totalHP, totalATK, totalDEF, specialBonus;
            levelData.CalculateTotalStatsBonus(out totalHP, out totalATK, out totalDEF, out specialBonus);
            int bonus = levelData.specialStatType == SpecialStatType.HP ? specialBonus : 0;
            return baseMaxHP + totalHP + bonus;
        }
    }

    public int atk
    {
        get
        {
            int totalHP, totalATK, totalDEF, specialBonus;
            levelData.CalculateTotalStatsBonus(out totalHP, out totalATK, out totalDEF, out specialBonus);
            int bonus = levelData.specialStatType == SpecialStatType.ATK ? specialBonus : 0;
            return baseAtk + totalATK + bonus;
        }
    }

    public int def
    {
        get
        {
            int totalHP, totalATK, totalDEF, specialBonus;
            levelData.CalculateTotalStatsBonus(out totalHP, out totalATK, out totalDEF, out specialBonus);
            int bonus = levelData.specialStatType == SpecialStatType.DEF ? specialBonus : 0;
            return baseDef + totalDEF + bonus;
        }
    }

    public int crit
    {
        get
        {
            int totalHP, totalATK, totalDEF, specialBonus;
            levelData.CalculateTotalStatsBonus(out totalHP, out totalATK, out totalDEF, out specialBonus);
            int bonus = levelData.specialStatType == SpecialStatType.Crit ? specialBonus : 0;
            return baseCrit + bonus;
        }
    }

    public int critDam
    {
        get
        {
            int totalHP, totalATK, totalDEF, specialBonus;
            levelData.CalculateTotalStatsBonus(out totalHP, out totalATK, out totalDEF, out specialBonus);
            int bonus = levelData.specialStatType == SpecialStatType.CritDam ? specialBonus : 0;
            return baseCritDam + bonus;
        }
    }

    [Header("Audio Clips")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip healSound;
    public AudioClip shieldSound;
    public AudioClip castSound;

    // Level up method
    public bool LevelUp()
    {
        if (!levelData.CanLevelUp())
            return false;

        int expCost = levelData.GetExpCostForNextLevel();
        int goldCost = levelData.GetGoldCostForNextLevel();

        if (!PlayerResourceManager.Instance.CanAffordUpgrade(expCost, goldCost))
            return false;

        PlayerResourceManager.Instance.SpendExp(expCost);
        PlayerResourceManager.Instance.SpendGold(goldCost);
        levelData.currentLevel++;

        return true;
    }

    // Copy constructor
    public PlayerStats(PlayerStats other)
    {
        this.baseMaxHP = other.baseMaxHP;
        this.baseAtk = other.baseAtk;
        this.baseDef = other.baseDef;
        this.baseCrit = other.baseCrit;
        this.baseCritDam = other.baseCritDam;
        this.characterIcon = other.characterIcon;
        this.characterName = other.characterName;
        this.characterClass = other.characterClass;
        this.attackSound = other.attackSound;
        this.hurtSound = other.hurtSound;
        this.healSound = other.healSound;
        this.shieldSound = other.shieldSound;
        this.castSound = other.castSound;
        
        // Copy level data
        this.levelData = new CharacterLevelData();
        this.levelData.currentLevel = other.levelData.currentLevel;
        this.levelData.hpPerLevel = other.levelData.hpPerLevel;
        this.levelData.atkPerLevel = other.levelData.atkPerLevel;
        this.levelData.defPerLevel = other.levelData.defPerLevel;
        this.levelData.specialStatType = other.levelData.specialStatType;
        this.levelData.specialStatBonus = other.levelData.specialStatBonus;
        this.levelData.baseExpCost = other.levelData.baseExpCost;
        this.levelData.baseGoldCost = other.levelData.baseGoldCost;
        this.levelData.costMultiplierPerLevel = other.levelData.costMultiplierPerLevel;
    }

    public PlayerStats() { }
}
