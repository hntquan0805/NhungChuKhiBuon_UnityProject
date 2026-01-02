using UnityEngine;

[CreateAssetMenu(fileName = "New Continuous Heal Effect", menuName = "Card Effects/Continuous Heal")]
public class ContinuousHealEffect : CardEffect
{
    [Header("Immediate Heal")]
    [Range(0f, 200f)]
    public float immediateHealPercent = 30f; // % HP của caster để heal ngay
    
    [Header("Buff Settings")]
    public int buffStacks = 3; // Số turn buff tồn tại
    [Range(0f, 100f)]
    public float healPerTurnPercent = 10f; // % HP của caster để heal mỗi turn
    public Sprite buffIcon; // Icon cho buff UI
    
    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        if (player == null)
        {
            Debug.LogError("ContinuousHealEffect: Missing player!");
            return;
        }
        
        // Lấy PlayerTeam từ player
        PlayerTeam team = player.GetComponentInParent<PlayerTeam>();
        if (team == null)
        {
            Debug.LogError("ContinuousHealEffect: Cannot find PlayerTeam!");
            return;
        }
        
        // 1. Heal toàn đội ngay lập tức (30% HP của caster)
        int immediateHeal = Mathf.RoundToInt(player.GetMaxHP() * immediateHealPercent / 100f);
        foreach (var teamPlayer in team.players)
        {
            if (teamPlayer != null && teamPlayer.GetCurrentHP() > 0)
            {
                teamPlayer.Heal(immediateHeal);
            }
        }
        
        // 2. Áp buff Continuous Heal (heal 10% HP/turn trong 3 turn)
        // Buff này sẽ được process trong ProcessBuffsAtTurnStart() của mỗi player
        foreach (var teamPlayer in team.players)
        {
            if (teamPlayer != null && teamPlayer.GetCurrentHP() > 0)
            {
                teamPlayer.AddBuff(BuffType.ContinuousHeal, buffStacks, buffIcon, player.GetMaxHP(), healPerTurnPercent);
            }
        }
        
        // Update team buff UI
        TeamBuffManager buffManager = team.GetComponent<TeamBuffManager>();
        if (buffManager != null)
        {
            buffManager.UpdateBuffUI();
        }
        
        // Trigger animation Cast
        player.PlayCast("Continuous Heal");
    }
}
