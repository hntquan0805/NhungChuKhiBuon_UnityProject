using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Heal (HP Scale)")]
public class HealHpScaleEffect : CardEffect
{
    [Header("Heal Calculation")]
    [Range(0, 500)]
    public int healPercent = 50; // % HP của player (50% = 0.5x HP)

    [Header("Fixed Heal Amount")]
    public int fixedHealAmount = 0; // Heal cố định

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Tính heal amount dựa trên MaxHP
        int playerMaxHP = player.GetMaxHP();
        int calculatedHeal = Mathf.RoundToInt(playerMaxHP * healPercent / 100f) + fixedHealAmount;

        Debug.Log($"[Heal Card - HP Scale] {player.name} using {healPercent}% HP ({playerMaxHP}) + {fixedHealAmount} = {calculatedHeal} heal");

        // Player sở hữu card: Heal CÓ ANIMATION
        player.Heal(calculatedHeal);

        // Heal cho các player còn lại KHÔNG CÓ ANIMATION
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            foreach (var teamPlayer in team.players)
            {
                if (teamPlayer != player) // Bỏ qua player đã heal với animation
                {
                    teamPlayer.HealSilent(calculatedHeal);
                }
            }
        }
    }
}
