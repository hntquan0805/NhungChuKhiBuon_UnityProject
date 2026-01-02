using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Increase Attack")]
public class IncreaseAttackEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "Battle Cry";

    [Header("Buff Settings")]
    public int buffStacks = 3; // Số stack buff (3 turns)
    public Sprite buffIcon; // Icon của buff

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Apply buff cho CẢ TEAM
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            foreach (var teamPlayer in team.players)
            {
                if (teamPlayer != null && teamPlayer.GetCurrentHP() > 0)
                {
                    teamPlayer.AddBuff(BuffType.IncreaseAttack, buffStacks, buffIcon);
                }
            }
        }

        // Trigger animation Cast
        player.PlayCast(castName);
    }
}
