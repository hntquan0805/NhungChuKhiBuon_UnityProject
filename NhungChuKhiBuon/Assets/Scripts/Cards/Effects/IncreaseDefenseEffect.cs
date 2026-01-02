using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Increase Defense")]
public class IncreaseDefenseEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "Shield Wall";

    [Header("Buff Settings")]
    public int buffStacks = 3; // Số stack buff (3 turns)
    public Sprite buffIcon; // Icon của buff

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        Debug.Log($"[Increase Defense Card] {player.name} cast {castName}: Apply {buffStacks} Increase Defense to all team members");

        // Apply buff cho CẢ TEAM
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            foreach (var teamPlayer in team.players)
            {
                if (teamPlayer != null && teamPlayer.GetCurrentHP() > 0)
                {
                    teamPlayer.AddBuff(BuffType.IncreaseDefense, buffStacks, buffIcon);
                }
            }
        }

        // Trigger animation Cast
        player.PlayCast(castName);
    }
}
