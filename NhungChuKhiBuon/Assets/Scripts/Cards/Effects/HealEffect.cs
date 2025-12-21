using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Heal")]
public class HealEffect : CardEffect
{
    public int healAmount = 10;

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Debug: Kiểm tra HP trước khi heal
        Debug.Log($"[HEAL] {player.name} - HP trước: {player.GetCurrentHP()}");

        // Player sở hữu card: Heal CÓ ANIMATION
        player.Heal(healAmount);

        // Debug: Kiểm tra HP sau khi heal
        Debug.Log($"[HEAL] {player.name} - HP sau: {player.GetCurrentHP()}");

        // Heal cho các player còn lại KHÔNG CÓ ANIMATION
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            foreach (var teamPlayer in team.players)
            {
                if (teamPlayer != player) // Bỏ qua player đã heal với animation
                {
                    Debug.Log($"[HEAL SILENT] {teamPlayer.name} - HP trước: {teamPlayer.GetCurrentHP()}");
                    teamPlayer.HealSilent(healAmount);
                    Debug.Log($"[HEAL SILENT] {teamPlayer.name} - HP sau: {teamPlayer.GetCurrentHP()}");
                }
            }
        }

        // Debug: Kiểm tra tổng HP team
        if (team != null)
        {
            Debug.Log($"[HEAL] Total Team HP: {team.GetTotalCurrentHP()}/{team.GetTotalMaxHP()}");
        }
    }
}