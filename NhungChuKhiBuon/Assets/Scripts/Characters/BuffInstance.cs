using UnityEngine;

[System.Serializable]
public class BuffInstance
{
    public BuffType type;
    public int stacks; // Số lượng stack
    public Sprite icon; // Icon của buff
    public int casterMaxHP; // MaxHP của người thi triển (cho Continuous Heal)
    public float healPercentage; // % heal mỗi turn (cho Continuous Heal)

    public BuffInstance(BuffType type, int stacks, Sprite icon = null, int casterMaxHP = 0, float healPercent = 10f)
    {
        this.type = type;
        this.stacks = stacks;
        this.icon = icon;
        this.casterMaxHP = casterMaxHP;
        this.healPercentage = healPercent;
    }

    public void AddStacks(int amount)
    {
        stacks += amount;
    }

    public void ReduceStacks(int amount)
    {
        stacks -= amount;
        if (stacks < 0) stacks = 0;
    }

    public float GetAttackBonus()
    {
        // IncreaseAttack: Tăng 40% ATK (stack chỉ là duration)
        if (type == BuffType.IncreaseAttack)
        {
            return 0.4f; // 40% cố định
        }
        return 0f;
    }

    public float GetDefenseBonus()
    {
        // IncreaseDefense: Tăng 60% DEF (stack chỉ là duration)
        if (type == BuffType.IncreaseDefense)
        {
            return 0.6f; // 60% cố định
        }
        return 0f;
    }
    
    public int GetHealAmount()
    {
        // ContinuousHeal: Hồi healPercentage% HP của caster mỗi turn
        if (type == BuffType.ContinuousHeal)
        {
            return Mathf.RoundToInt(casterMaxHP * healPercentage / 100f);
        }
        return 0;
    }
}
