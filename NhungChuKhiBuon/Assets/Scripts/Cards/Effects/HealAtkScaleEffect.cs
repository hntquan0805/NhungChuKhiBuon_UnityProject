using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Heal (ATK Scale)")]
public class HealAtkScaleEffect : CardEffect
{
    [Header("Heal Calculation")]
    [Range(0, 500)]
    public int healPercent = 50; // % ATK của player (50% = 0.5x ATK)

    [Header("Fixed Heal Amount")]
    public int fixedHealAmount = 0; // Heal cố định

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Tính heal amount dựa trên ATK
        int playerATK = player.GetATK();
        int calculatedHeal = Mathf.RoundToInt(playerATK * healPercent / 100f) + fixedHealAmount;

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
