using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Shield (HP Scale)")]
public class ShieldHpScaleEffect : CardEffect
{
    [Header("Shield Calculation")]
    [Range(0, 500)]
    public int shieldPercent = 100; // % HP của player (100% = 1.0x HP)

    [Header("Optional Fixed Shield")]
    public int fixedShield = 0; // Shield cố định thêm vào

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Tính shield dựa trên MaxHP của player
        int playerMaxHP = player.GetMaxHP();
        int calculatedShield = Mathf.RoundToInt(playerMaxHP * shieldPercent / 100f) + fixedShield;

        Debug.Log($"[Shield Card - HP Scale] {player.name} using {shieldPercent}% HP ({playerMaxHP}) + {fixedShield} = {calculatedShield} shield");

        // Chỉ player sở hữu card mới trigger animation
        player.PlayShield(calculatedShield);
    }
}
