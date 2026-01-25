using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Execute Cast")]
public class ExecuteCastEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "Execute";

    [Header("HP Cost")]
    [Range(0, 100)]
    public int hpCostPercent = 20; // % HP hiện tại tiêu tốn (20%)

    [Header("Damage Calculation")]
    [Range(0, 500)]
    public int atkDamagePercent = 75; // % ATK của player (75%)
    [Range(0, 100)]
    public int targetMaxHPPercent = 10; // % Max HP của enemy (10%)

    [Header("Defense Penetration")]
    [Range(0, 100)]
    public int defenseIgnorePercent = 30; // % phòng ngự bỏ qua (30%)

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Kiểm tra enemy có hợp lệ không
        if (enemy == null)
        {
            Debug.LogWarning("ExecuteCastEffect: No valid enemy target!");
            return;
        }

        if (enemy.GetCurrentHP() <= 0)
        {
            Debug.LogWarning("ExecuteCastEffect: Target enemy is already dead!");
            return;
        }

        // ===== TRACKING ATTACKER CHO BOSS =====
        BossEnemyLevel1 boss = enemy as BossEnemyLevel1;
        if (boss != null)
        {
            boss.SetLastAttacker(player);
        }

        // 1. Tiêu tốn 20% HP hiện tại của player
        int hpCost = Mathf.RoundToInt(player.GetCurrentHP() * hpCostPercent / 100f);
        player.TakeDamage(hpCost);

        Debug.Log($"[Execute Cast] {player.name} spent {hpCost} HP ({hpCostPercent}% of current HP)");

        // 2. Tính damage
        int playerATK = player.GetATK();
        int enemyMaxHP = enemy.GetMaxHP();

        // Damage = 75% ATK + 10% Max HP của enemy
        int atkDamage = Mathf.RoundToInt(playerATK * atkDamagePercent / 100f);
        int hpDamage = Mathf.RoundToInt(enemyMaxHP * targetMaxHPPercent / 100f);
        int calculatedDamage = atkDamage + hpDamage;

        // Giới hạn damage không vượt quá 300% ATK của player
        int maxDamage = Mathf.RoundToInt(playerATK * 3f); // 300% ATK
        int baseDamage = Mathf.Min(calculatedDamage, maxDamage);

        Debug.Log($"[Execute Cast] Calculated: {atkDamage} (ATK) + {hpDamage} (Enemy Max HP) = {calculatedDamage}");
        Debug.Log($"[Execute Cast] Base Damage after cap: {baseDamage} (max: {maxDamage})");

        // Set target
        player.SetTarget(enemy);

        // 3. Áp dụng defense penetration TRƯỚC KHI tính crit (bỏ qua 30% phòng ngự)
        int enemyDefense = enemy.GetDefense();
        int effectiveDefense = Mathf.RoundToInt(enemyDefense * (1f - defenseIgnorePercent / 100f));

        // Công thức giảm damage theo defense: damage * (damage / (damage + defense))
        int damageAfterDefense = Mathf.RoundToInt(baseDamage * (baseDamage / (float)(baseDamage + effectiveDefense)));
        damageAfterDefense = Mathf.Max(damageAfterDefense, 0);

        Debug.Log($"[Execute Cast] Defense Penetration: {enemyDefense} → {effectiveDefense} (ignored {defenseIgnorePercent}%)");
        Debug.Log($"[Execute Cast] Damage after defense: {damageAfterDefense}");

        // 4. Tính crit và class advantage SAU KHI áp dụng defense
        DamageResult damageResult = player.CalculateDamage(damageAfterDefense, enemy);

        Debug.Log($"[Execute Cast] Final Damage: {damageResult.finalDamage} (Crit: {damageResult.isCritical})");

        // 5. Gây damage ngay lập tức
        enemy.TakeDamage(damageResult.finalDamage);

        // Trigger animation Cast
        player.PlayCast(castName);
    }
}