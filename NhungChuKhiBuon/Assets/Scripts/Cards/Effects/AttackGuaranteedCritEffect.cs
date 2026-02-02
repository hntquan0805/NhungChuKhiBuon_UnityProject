using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Attack (Guaranteed Crit)")]
public class AttackGuaranteedCritEffect : CardEffect
{
    [Header("Damage Calculation")]
    [Range(0, 500)]
    public int damagePercent = 75; // % ATK của player (75% = 0.75x ATK)

    [Header("Optional Fixed Damage")]
    public int fixedDamage = 0; // Damage cố định thêm vào

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Kiểm tra enemy có hợp lệ không
        if (enemy == null)
        {
            Debug.LogWarning("AttackGuaranteedCritEffect: No valid enemy target!");
            return;
        }

        if (enemy.GetCurrentHP() <= 0)
        {
            Debug.LogWarning("AttackGuaranteedCritEffect: Target enemy is already dead!");
            return;
        }

        // ===== TRACKING ATTACKER CHO BOSS =====
        BossEnemyLevel1 boss = enemy as BossEnemyLevel1;
        if (boss != null)
        {
            boss.SetLastAttacker(player);
        }

        // Tính damage dựa trên ATK của player
        int playerATK = player.GetATK();
        int baseDamage = Mathf.RoundToInt(playerATK * damagePercent / 100f) + fixedDamage;

        // Set target
        player.SetTarget(enemy);

        // Tính guaranteed crit damage
        int playerCritDam = player.GetCritDam();
        int criticalDamage = Mathf.RoundToInt(baseDamage * playerCritDam / 100f);

        // Apply class advantage
        float classMultiplier = ClassAdvantage.GetDamageMultiplier(player.stats.characterClass, enemy.stats.characterClass);
        int finalDamage = Mathf.RoundToInt(criticalDamage * classMultiplier);

        Debug.Log($"[Guaranteed Crit Attack] {player.name}: {baseDamage} base → {criticalDamage} crit ({playerCritDam}%) → {finalDamage} final (class: x{classMultiplier})");

        // Gây damage trực tiếp
        enemy.TakeDamage(finalDamage);

        // Trigger animation attack (chỉ để visual)
        player.PlayAttack();
    }
}