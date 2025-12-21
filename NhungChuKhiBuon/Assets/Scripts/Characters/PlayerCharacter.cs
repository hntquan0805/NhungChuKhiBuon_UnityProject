using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    [Header("Player Stats")]
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private int defense = 10; // Defense riêng của player
    [SerializeField] private int shieldAmount = 5;

    private EnemyCharacter targetEnemy;

    public void SetTarget(EnemyCharacter enemy)
    {
        targetEnemy = enemy;
    }

    // Triggered khi nhấn card Attack
    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    // Animation Event gọi khi đánh trúng
    public void DealDamage()
    {
        if (targetEnemy != null)
        {
            int hpBefore = targetEnemy.GetCurrentHP();
            targetEnemy.TakeDamage(attackDamage);
            int hpAfter = targetEnemy.GetCurrentHP();
            int actualDamage = hpBefore - hpAfter;

            Debug.Log($"💥 {gameObject.name} dealt {actualDamage} damage to {targetEnemy.gameObject.name} (Base: {attackDamage})");
        }
    }

    // Triggered khi nhấn card Heal
    public void PlayHealCard(int amount)
    {
        Heal(amount);
    }

    // Triggered khi nhấn card Shield - CÓ ANIMATION
    public void PlayShield(int amount)
    {
        shieldAmount += amount;
        if (animator != null)
            animator.SetTrigger("Shield");
    }

    // Thêm shield KHÔNG CÓ ANIMATION (dùng cho team buff)
    public void AddShieldSilent(int amount)
    {
        shieldAmount += amount;
    }

    // Triggered khi nhấn card Cast (ví dụ skill)
    public void PlayCast(string castName)
    {
        if (animator != null)
            animator.SetTrigger("Cast");
        Debug.Log("Cast skill: " + castName);
        // Thêm logic skill sau này
    }

    public int GetShieldAmount()
    {
        return shieldAmount;
    }

    public void ReduceShield(int amount)
    {
        shieldAmount -= amount;
        shieldAmount = Mathf.Max(shieldAmount, 0);
    }

    public int GetDefense()
    {
        return defense;
    }

    public override void Heal(int amount)
    {
        base.Heal(amount); // Gọi base để update HP đúng cách
    }

    // Heal KHÔNG CÓ ANIMATION (dùng cho team buff)
    public void HealSilent(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    // Public method để trigger hurt animation
    public new void PlayHurt()
    {
        base.PlayHurt();
    }

    public int GetMaxHP()
    {
        return maxHP;
    }
}