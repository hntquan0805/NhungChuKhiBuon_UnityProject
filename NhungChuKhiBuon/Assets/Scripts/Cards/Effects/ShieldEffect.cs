using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Shield")]
public class ShieldEffect : CardEffect
{
    public int shieldAmount = 8;

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // CHỈ player sở hữu card mới trigger animation
        player.PlayShield(shieldAmount);

        // Apply shield cho các player còn lại KHÔNG CÓ ANIMATION
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            foreach (var teamPlayer in team.players)
            {
                if (teamPlayer != player) // Bỏ qua player đã trigger animation
                {
                    teamPlayer.AddShieldSilent(shieldAmount);
                }
            }
        }
    }
}