using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Attack (ATK Scale)")]
public class AttackAtkScaleEffect : CardEffect
{
    [Header("Damage Calculation")]
    [Range(0, 500)]
    public int damagePercent = 100; // % ATK của player (100% = 1.0x ATK)

    [Header("Optional Fixed Damage")]
    public int fixedDamage = 0; // Damage cố định thêm vào

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Kiểm tra enemy có hợp lệ không
        if (enemy == null)
        {
            Debug.LogWarning("AttackAtkScaleEffect: No valid enemy target!");
            return;
        }

        if (enemy.GetCurrentHP() <= 0)
        {
            Debug.LogWarning("AttackAtkScaleEffect: Target enemy is already dead!");
            return;
        }

        // Tính damage dựa trên ATK của player
        int playerATK = player.GetATK();
        int calculatedDamage = Mathf.RoundToInt(playerATK * damagePercent / 100f) + fixedDamage;

        // Set target
        player.SetTarget(enemy);

        // Lưu damage tạm vào component để DealDamage() có thể truy cập
        TempDamageHolder holder = player.gameObject.GetComponent<TempDamageHolder>();
        if (holder == null)
        {
            holder = player.gameObject.AddComponent<TempDamageHolder>();
        }
        holder.damage = calculatedDamage;

        // Trigger animation attack của player này
        player.PlayAttack();

        // DealDamage sẽ được gọi từ Animation Event
        // Animation Event sẽ tính critical và apply damage cuối cùng
    }
}
