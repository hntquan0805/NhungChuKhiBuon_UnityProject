using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    [Header("Base Stats")]
    public int maxHP = 100;
    public int atk = 50;        // Sát thương cơ bản
    public int def = 10;        // Phòng thủ (%)

    [Header("Critical Stats")]
    [Range(0, 100)]
    public int crit = 15;       // Tỉ lệ chí mạng (%)
    [Range(100, 300)]
    public int critDam = 150;   // Sát thương chí mạng (%)

    [Header("Class")]
    public CharacterClass characterClass = CharacterClass.Knight;

    [Header("Audio Clips")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip healSound;
    public AudioClip shieldSound;
    public AudioClip castSound;

    // Copy constructor
    public PlayerStats(PlayerStats other)
    {
        this.maxHP = other.maxHP;
        this.atk = other.atk;
        this.def = other.def;
        this.crit = other.crit;
        this.critDam = other.critDam;
        this.characterClass = other.characterClass;
        this.attackSound = other.attackSound;
        this.hurtSound = other.hurtSound;
        this.healSound = other.healSound;
        this.shieldSound = other.shieldSound;
        this.castSound = other.castSound;
    }

    public PlayerStats() { }
}
