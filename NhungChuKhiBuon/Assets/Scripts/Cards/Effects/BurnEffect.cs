using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Burn Effect")]
public class BurnEffect : CardEffect
{
    [Header("Cast Settings")]
    public string castName = "Fire Blast";

    [Header("Damage Calculation")]
    [Range(0, 200)]
    public int damagePercent = 50; // % ATK (mặc định 50%)

    [Header("Burn Settings")]
    public int burnStacks = 2; // Số stack Burn sẽ apply
    [Range(0, 200)]
    public int burnDamagePercent = 75; // % ATK cho mỗi tick Burn (mặc định 75%)
    public Sprite burnIcon; // Icon của Burn debuff

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Kiểm tra enemy có hợp lệ không
        if (enemy == null)
        {
            Debug.LogWarning("BurnEffect: No valid enemy target!");
            return;
        }

        if (enemy.GetCurrentHP() <= 0)
        {
            Debug.LogWarning("BurnEffect: Target enemy is already dead!");
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
        int baseDamage = Mathf.RoundToInt(playerATK * damagePercent / 100f);

        // Set target
        player.SetTarget(enemy);

        // Tính damage với crit và class advantage
        DamageResult damageResult = player.CalculateDamage(baseDamage, enemy);

        // Apply damage ngay lập tức
        enemy.TakeDamage(damageResult.finalDamage);

        // Apply Burn debuff
        enemy.AddDebuff(DebuffType.Burn, burnStacks, player, burnIcon);

        // Trigger animation Cast
        player.PlayCast(castName);
    }
}
