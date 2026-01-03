using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Decrease Attack")]
public class DecreaseAttackEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "Weaken";

    [Header("Debuff Settings")]
    public int debuffStacks = 2; // Số stack Decrease Attack
    public Sprite debuffIcon; // Icon của debuff

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Lấy tất cả enemies từ BattleManager
        if (BattleManager.Instance == null || BattleManager.Instance.enemies.Count == 0)
        {
            Debug.LogWarning("DecreaseAttackEffect: No enemies in battle!");
            return;
        }

        Debug.Log($"[Decrease Attack Card] {player.name} cast {castName}: Apply {debuffStacks} Decrease Attack stacks to ALL enemies");

        // Apply Decrease Attack debuff cho tất cả enemies
        foreach (var targetEnemy in BattleManager.Instance.enemies)
        {
            if (targetEnemy != null && targetEnemy.GetCurrentHP() > 0)
            {
                targetEnemy.AddDebuff(DebuffType.DecreaseAttack, debuffStacks, player, debuffIcon);
                Debug.Log($"[DEBUFF] Applied Decrease Attack to {targetEnemy.name}");
            }
        }

        // Trigger animation Cast
        player.PlayCast(castName);
    }
}
