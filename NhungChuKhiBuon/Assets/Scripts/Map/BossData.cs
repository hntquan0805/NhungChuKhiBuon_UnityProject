using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Game/Boss Data")]
public class BossData : ScriptableObject
{
    [Header("Thông tin hiển thị")]
    public string bossName;
    public GameObject bossPrefab; // Prefab con boss (đã gắn AI, Animation...)
    public Sprite bossIcon;

    [Header("Chỉ số")]
    public float maxHP;
    public int damage;

    [Header("Điều hướng")]
    // Tên của Scene Map tiếp theo sau khi thắng Boss này
    // Ví dụ Boss 1 -> "MapLevel2", Boss 3 -> "EndGame"
    public string nextMapSceneName;
}