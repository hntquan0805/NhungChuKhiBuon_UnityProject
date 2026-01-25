using UnityEngine;

public class FlatEnemy : EnemyCharacter
{
    [Header("Flat Settings")]
    [SerializeField] private float lifestealPercent = 25f; // Hồi máu 25% sát thương gây ra
    
    // Override để không có passive tăng attack từ base class
    protected override void ApplyStartPassive()
    {
        // Flat không có passive nội tại
    }
    
    protected override void ApplyTurnPassive()
    {
        // Flat không có passive nội tại
    }
    
    // Override DealDamage để thêm lifesteal
    public override void DealDamage()
    {
        Debug.Log($"[FlatEnemy] DealDamage called for {gameObject.name}");
        if (targetTeam == null)
        {
            return;
        }

        // Tính damage dựa trên stats
        int baseDamage = Mathf.RoundToInt(stats.atk * stats.attackPercent / 100f);

        // Tính critical
        EnemyDamageResult damageResult = CalculateDamage(baseDamage);

        // Áp dụng defense của team
        int teamDefense = targetTeam.GetTotalDefense();
        int actualDamage = Mathf.RoundToInt(damageResult.finalDamage * (damageResult.finalDamage / (float)(damageResult.finalDamage + teamDefense)));
        actualDamage = Mathf.Max(actualDamage, 0);

        // Lưu lại damage thực tế để tính lifesteal
        int totalDamageDealt = actualDamage;

        int teamShield = targetTeam.GetTeamShield();
        int remainingDamage = actualDamage;

        if (teamShield > 0)
        {
            int shieldToAbsorb = Mathf.Min(teamShield, actualDamage);
            remainingDamage -= shieldToAbsorb;

            // Giảm shield của team
            targetTeam.ReduceShield(shieldToAbsorb);
        }

        // Chia damage cho TẤT CẢ players còn sống
        if (remainingDamage > 0)
        {
            int playersAlive = 0;
            foreach (var player in targetTeam.players)
            {
                if (player.GetCurrentHP() > 0)
                    playersAlive++;
            }

            if (playersAlive > 0)
            {
                int damagePerPlayer = Mathf.CeilToInt((float)remainingDamage / playersAlive);

                foreach (var player in targetTeam.players)
                {
                    if (player.GetCurrentHP() > 0)
                    {
                        player.TakeDamage(damagePerPlayer);
                    }
                }
            }
        }
        else
        {
            // Shield block hết damage -> chỉ play hurt animation
            foreach (var player in targetTeam.players)
            {
                if (player.GetCurrentHP() > 0)
                {
                    player.PlayHurt();
                }
            }
        }

        // Lifesteal: Hồi máu 25% sát thương đã gây ra
        int healAmount = Mathf.RoundToInt(totalDamageDealt * lifestealPercent / 100f);
        if (healAmount > 0)
        {
            Heal(healAmount);
            Debug.Log($"[FlatEnemy] Lifesteal: healed {healAmount} HP from {totalDamageDealt} damage dealt");
        }
    }
}
