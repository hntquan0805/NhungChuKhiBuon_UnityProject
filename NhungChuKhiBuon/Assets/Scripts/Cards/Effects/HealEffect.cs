using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Heal")]
public class HealEffect : CardEffect
{
    [Header("Heal Calculation")]
    [Range(0, 500)]
    public int healPercent = 0; // % ATK của player (0% = không dùng ATK)

    [Header("Fixed Heal Amount")]
    public int fixedHealAmount = 20; // Heal cố định

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Tính heal amount
        int playerHP = player.GetMaxHP();
        int calculatedHeal = Mathf.RoundToInt(playerHP * healPercent / 100f) + fixedHealAmount;

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