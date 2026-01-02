using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : CharacterBase
{
    [Header("Player Stats")]
    public PlayerStats stats = new PlayerStats();

    private EnemyCharacter targetEnemy;
    
    // Buff system
    private List<BuffInstance> buffs = new List<BuffInstance>();

    protected override void Awake()
    {
        maxHP = stats.maxHP;
        base.Awake();
    }

    public void SetTarget(EnemyCharacter enemy)
    {
        targetEnemy = enemy;
    }

    public void SetHP(int current, int max)
    {
        maxHP = max;
        currentHP = current;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (targetEnemy != null)
        {
            int baseDamage = GetComponent<TempDamageHolder>()?.damage ?? 0;
            DamageResult result = CalculateDamage(baseDamage, targetEnemy);

            targetEnemy.TakeDamage(result.finalDamage);

            Destroy(GetComponent<TempDamageHolder>());
        }
    }

    public DamageResult CalculateDamage(int baseDamage, EnemyCharacter target)
    {
        DamageResult result = new DamageResult();
        result.rawDamage = baseDamage;
        result.isCritical = Random.Range(0, 100) < stats.crit;

        if (result.isCritical)
        {
            result.finalDamage = Mathf.RoundToInt(result.rawDamage * stats.critDam / 100f);
        }
        else
        {
            result.finalDamage = result.rawDamage;
        }

        if (target != null)
        {
            float classMultiplier = ClassAdvantage.GetDamageMultiplier(stats.characterClass, target.stats.characterClass);
            result.finalDamage = Mathf.RoundToInt(result.finalDamage * classMultiplier);
            result.hasClassAdvantage = classMultiplier > 1.0f;
        }

        return result;
    }

    public void PlayHealCard(int amount)
    {
        Heal(amount);
    }

    public void PlayShield(int amount)
    {
        if (animator != null)
            animator.SetTrigger("Shield");

        // Thêm shield vào team
        PlayerTeam team = GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            team.AddShield(amount);
        }
    }

    public void PlayCast(string castName)
    {
        if (animator != null)
            animator.SetTrigger("Cast");
    }

    public int GetDefense()
    {
        int baseDEF = stats.def;
        
        // Tính tổng tăng DEF từ buff (phần trăm)
        float totalBonus = 0f;
        foreach (var buff in buffs)
        {
            totalBonus += buff.GetDefenseBonus();
        }
        
        int boostedDEF = Mathf.RoundToInt(baseDEF * (1f + totalBonus));
        
        return boostedDEF;
    }

    public int GetATK()
    {
        int baseATK = stats.atk;
        
        // Tính tổng tăng ATK từ buff
        float totalBonus = 0f;
        foreach (var buff in buffs)
        {
            totalBonus += buff.GetAttackBonus();
        }
        
        int boostedATK = Mathf.RoundToInt(baseATK * (1f + totalBonus));
        
        return boostedATK;
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

    public void HealSilent(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public new void PlayHurt()
    {
        base.PlayHurt();
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    // ========== BUFF SYSTEM ==========
    
    public void AddBuff(BuffType type, int stacks, Sprite icon = null, int casterMaxHP = 0, float healPercent = 10f)
    {
        // Luôn cộng stack nếu đã có buff cùng loại
        BuffInstance existingBuff = buffs.Find(b => b.type == type);
        
        if (existingBuff != null)
        {
            existingBuff.AddStacks(stacks);
        }
        else
        {
            BuffInstance newBuff = new BuffInstance(type, stacks, icon, casterMaxHP, healPercent);
            buffs.Add(newBuff);
        }
        
        // Update team buff UI
        UpdateTeamBuffUI();
    }
    
    public void RemoveBuff(BuffType type)
    {
        buffs.RemoveAll(b => b.type == type);
        UpdateTeamBuffUI();
    }
    
    public void ProcessBuffsAtTurnStart()
    {
        List<BuffInstance> buffsToRemove = new List<BuffInstance>();
        
        foreach (var buff in buffs)
        {
            // 1. Áp dụng effect trước khi giảm stack
            if (buff.type == BuffType.ContinuousHeal)
            {
                // Heal toàn đội 10% HP của caster
                int healAmount = buff.GetHealAmount();
                HealWholeTeam(healAmount);
            }
            
            // 2. Giảm 1 stack
            buff.ReduceStacks(1);
            
            // 3. Nếu hết stack thì đánh dấu để xóa
            if (buff.stacks <= 0)
            {
                buffsToRemove.Add(buff);
            }
        }
        
        // Xóa các buff đã hết stack
        foreach (var buff in buffsToRemove)
        {
            buffs.Remove(buff);
        }
        
        // LUÔN update UI sau khi process buffs
        if (buffs.Count > 0 || buffsToRemove.Count > 0)
        {
            UpdateTeamBuffUI();
        }
    }
    
    private void HealWholeTeam(int amount)
    {
        // Tìm PlayerTeam trong scene và heal toàn đội
        PlayerTeam team = FindObjectOfType<PlayerTeam>();
        if (team != null)
        {
            foreach (var player in team.players)
            {
                if (player != null && player.GetCurrentHP() > 0)
                {
                    player.Heal(amount);
                }
            }
        }
    }
    
    public List<BuffInstance> GetBuffs()
    {
        return buffs;
    }
    
    private void UpdateTeamBuffUI()
    {
        // Gọi team buff manager để update UI
        PlayerTeam team = GetComponentInParent<PlayerTeam>();
        if (team != null)
        {
            TeamBuffManager buffManager = team.GetComponent<TeamBuffManager>();
            if (buffManager != null)
            {
                buffManager.UpdateBuffUI();
            }
        }
    }
    
    public void ClearBuffs()
    {
        buffs.Clear();
        UpdateTeamBuffUI();
    }
}

[System.Serializable]
public struct DamageResult
{
    public int rawDamage;
    public int finalDamage;
    public bool isCritical;
    public bool hasClassAdvantage;
}

public class TempDamageHolder : MonoBehaviour
{
    public int damage;
}