using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Shield (ATK Scale)")]
public class ShieldAtkScaleEffect : CardEffect
{
    [Header("Shield Calculation")]
    [Range(0, 500)]
    public int shieldPercent = 100; // % ATK của player (100% = 1.0x ATK)

    [Header("Optional Fixed Shield")]
    public int fixedShield = 0; // Shield cố định thêm vào

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Tính shield dựa trên ATK của player
        int playerATK = player.GetATK();
        int calculatedShield = Mathf.RoundToInt(playerATK * shieldPercent / 100f) + fixedShield;

        // Chỉ player sở hữu card mới trigger animation
        player.PlayShield(calculatedShield);
    }
}
