using UnityEngine;

public class BossEnemyLevel1 : EnemyCharacter
{
    [Header("Boss Heal Settings")]
    [SerializeField] private int healCooldown = 3;
    [SerializeField] private float instantHealPercent = 20f;
    [SerializeField] private int continuousHealDuration = 2;
    [SerializeField] private float continuousHealPercent = 10f;
    [SerializeField] private Sprite continuousHealIcon;

    [Header("Boss Poison Settings")]
    [SerializeField] private int poisonStacksOnHit = 1;
    [SerializeField] private Sprite poisonIcon;

    private int healCooldownCounter = 0;
    private PlayerCharacter lastAttacker = null;

    // ===== TẮT PASSIVE GỐC =====
    protected override void ApplyStartPassive() { }
    protected override void ApplyTurnPassive() { }

    protected override void Awake()
    {
        base.Awake();
    }

    // ===== PASSIVE: BỊ ĐÁNH → GÂY POISON =====
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (amount <= 0) return;

        // Gắn poison cho player đang đánh boss
        if (lastAttacker != null && lastAttacker.GetCurrentHP() > 0)
        {
            Debug.Log(
                $"[BossEnemyLevel1] Add POISON to {lastAttacker.name} | " +
                $"Stacks: {poisonStacksOnHit}"
            );
            lastAttacker.AddDebuff(
                DebuffType.Poison,
                poisonStacksOnHit,
                this,
                poisonIcon
            );
        }

    }

    // Method để set attacker từ card effect
    public void SetLastAttacker(PlayerCharacter attacker)
    {
        lastAttacker = attacker;
    }

    // ===== OVERRIDE AI - BOSS HÀNH ĐỘNG KHI CP = 0 HOẶC END TURN =====
    public override void DealDamage()
    {
        Debug.Log($"[BossEnemyLevel1] DealDamage HP={currentHP}/{GetMaxHP()}, healCD={healCooldownCounter}");

        if (targetTeam == null) return;

        float hpPercent = (float)currentHP / GetMaxHP();

        // CHỈ HEAL KHI:
        // - cooldown = 0
        // - HP ≤ 90%
        if (healCooldownCounter <= 0 && hpPercent <= 0.9f)
        {
            Debug.Log("[BossEnemyLevel1] USE HEAL");

            UseHealSkill();
            healCooldownCounter = healCooldown;
            return;
        }

        // KHÔNG HEAL → TẤN CÔNG
        Debug.Log("[BossEnemyLevel1] NORMAL ATTACK");

        UseNormalAttack();

        // CHỈ GIẢM CD KHI KHÔNG HEAL
        if (healCooldownCounter > 0)
            healCooldownCounter--;
    }


    // ===== SKILL 1: ĐÁNH 90% ATK =====
    private void UseNormalAttack()
    {
        if (targetTeam == null) return;

        // Backup attack percent
        int originalAttackPercent = stats.attackPercent;
        stats.attackPercent = 90;

        // Play animation
        PlayAttack();

        // Calculate damage với 90% ATK
        int baseDamage = Mathf.RoundToInt(stats.atk * stats.attackPercent / 100f);
        EnemyDamageResult damageResult = CalculateDamage(baseDamage);

        // Apply defense
        int teamDefense = targetTeam.GetTotalDefense();
        int actualDamage = Mathf.RoundToInt(damageResult.finalDamage * (damageResult.finalDamage / (float)(damageResult.finalDamage + teamDefense)));
        actualDamage = Mathf.Max(actualDamage, 0);

        // Apply shield
        int teamShield = targetTeam.GetTeamShield();
        int remainingDamage = actualDamage;

        if (teamShield > 0)
        {
            int shieldToAbsorb = Mathf.Min(teamShield, actualDamage);
            remainingDamage -= shieldToAbsorb;
            targetTeam.ReduceShield(shieldToAbsorb);
        }

        // Chia damage cho tất cả players còn sống
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
            // Shield block hết damage → chỉ play hurt animation
            foreach (var player in targetTeam.players)
            {
                if (player.GetCurrentHP() > 0)
                {
                    player.PlayHurt();
                }
            }
        }

        // Restore attack percent
        stats.attackPercent = originalAttackPercent;
    }

    // ===== SKILL 2: HEAL + CONTINUOUS HEAL =====
    private void UseHealSkill()
    {
        // Play heal animation
        if (animator != null)
            animator.SetTrigger("Cast"); // Hoặc "Heal" nếu có

        // Instant heal
        int healAmount = Mathf.RoundToInt(GetMaxHP() * instantHealPercent / 100f);
        currentHP = Mathf.Min(currentHP + healAmount, GetMaxHP());

        // Continuous heal buff
        AddBuff(
            BuffType.ContinuousHeal,
            continuousHealDuration,
            continuousHealIcon
        );

        // Set heal parameters
        var buffs = GetBuffs();
        BuffInstance healBuff = buffs.Find(b => b.type == BuffType.ContinuousHeal);
        if (healBuff != null)
        {
            healBuff.casterMaxHP = GetMaxHP();
            healBuff.healPercentage = continuousHealPercent;
        }
    }
}