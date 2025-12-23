using UnityEngine;

[System.Serializable]
public class EnemyStats
{
    [Header("Base Stats")]
    public int maxHP = 100;
    public int atk = 40;        // Sát thương cơ bản
    public int def = 5;         // Phòng thủ (%) - có thể dùng sau

    [Header("Critical Stats")]
    [Range(0, 100)]
    public int crit = 10;       // Tỉ lệ chí mạng (%)
    [Range(100, 300)]
    public int critDam = 150;   // Sát thương chí mạng (%)

    [Header("Attack Multiplier")]
    [Range(0, 500)]
    public int attackPercent = 100; // % ATK khi tấn công (mặc định 100% = 1x ATK)

    [Header("Class")]
    public CharacterClass characterClass = CharacterClass.Mage;

    // Copy constructor
    public EnemyStats(EnemyStats other)
    {
        this.maxHP = other.maxHP;
        this.atk = other.atk;
        this.def = other.def;
        this.crit = other.crit;
        this.critDam = other.critDam;
        this.attackPercent = other.attackPercent;
        this.characterClass = other.characterClass;
    }

    public EnemyStats() { }
}