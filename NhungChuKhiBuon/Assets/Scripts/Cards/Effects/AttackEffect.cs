using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Attack")]
public class AttackEffect : CardEffect
{
    public int damage = 10;

    public override void Execute(PlayerCharacter player, EnemyCharacter enemy)
    {
        // Kiểm tra enemy có hợp lệ không
        if (enemy == null)
        {
            return;
        }

        if (enemy.GetCurrentHP() <= 0)
        {
            return;
        }

        // Set target
        player.SetTarget(enemy);

        // Trigger animation attack của player này
        player.PlayAttack();

        // DealDamage sẽ được gọi từ Animation Event
        // Hoặc nếu không có Animation Event, uncomment dòng dưới:
        // player.DealDamage();
    }
}