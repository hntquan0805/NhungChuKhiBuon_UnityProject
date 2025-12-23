using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    [Header("Player Stats")]
    public PlayerStats stats = new PlayerStats();

    [Header("Runtime Stats")]
    [SerializeField] private int shieldAmount = 0;

    private EnemyCharacter targetEnemy;

    protected override void Awake()
    {
        // Sử dụng maxHP từ stats
        maxHP = stats.maxHP;
        base.Awake();
    }

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

            // Lấy damage từ card effect (sẽ được set từ AttackEffect)
            int baseDamage = GetComponent<TempDamageHolder>()?.damage ?? 0;

            // Tính damage thực tế (bao gồm class advantage)
            DamageResult result = CalculateDamage(baseDamage, targetEnemy);

            targetEnemy.TakeDamage(result.finalDamage);

            int hpAfter = targetEnemy.GetCurrentHP();
            int actualDamage = hpBefore - hpAfter;

            string critText = result.isCritical ? " [CRITICAL HIT!]" : "";
            string classText = result.hasClassAdvantage ? " [CLASS ADVANTAGE!]" : "";
            Debug.Log($"💥 {gameObject.name} dealt {actualDamage} damage to {targetEnemy.gameObject.name}{critText}{classText} (Base: {baseDamage}, Calculated: {result.finalDamage})");

            // Clear temp damage
            Destroy(GetComponent<TempDamageHolder>());
        }
    }

    // Tính toán damage dựa trên stats
    public DamageResult CalculateDamage(int baseDamage, EnemyCharacter target)
    {
        DamageResult result = new DamageResult();

        // Damage = ATK * multiplier từ card
        result.rawDamage = baseDamage;

        // Check critical hit
        result.isCritical = Random.Range(0, 100) < stats.crit;

        if (result.isCritical)
        {
            // Áp dụng critical damage
            result.finalDamage = Mathf.RoundToInt(result.rawDamage * stats.critDam / 100f);
        }
        else
        {
            result.finalDamage = result.rawDamage;
        }

        // Áp dụng class advantage
        if (target != null)
        {
            float classMultiplier = ClassAdvantage.GetDamageMultiplier(stats.characterClass, target.stats.characterClass);
            result.finalDamage = Mathf.RoundToInt(result.finalDamage * classMultiplier);
            result.hasClassAdvantage = classMultiplier > 1.0f;
        }

        return result;
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
        return stats.def;
    }

    public int GetATK()
    {
        return stats.atk;
    }

    public int GetCrit()
    {
        return stats.crit;
    }

    public int GetCritDam()
    {
        return stats.critDam;
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
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

// Struct để trả kết quả damage calculation
[System.Serializable]
public struct DamageResult
{
    public int rawDamage;
    public int finalDamage;
    public bool isCritical;
    public bool hasClassAdvantage;
}

// Component tạm để lưu damage từ card effect
public class TempDamageHolder : MonoBehaviour
{
    public int damage;
}