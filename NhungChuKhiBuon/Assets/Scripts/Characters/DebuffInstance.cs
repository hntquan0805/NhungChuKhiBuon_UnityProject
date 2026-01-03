using UnityEngine;

[System.Serializable]
public class DebuffInstance
{
    public DebuffType type;
    public int stacks; // Số lượng stack
    public PlayerCharacter source; // Người gây ra debuff (để lấy ATK)
    public Sprite icon; // Icon của debuff

    public DebuffInstance(DebuffType type, int stacks, PlayerCharacter source, Sprite icon = null)
    {
        this.type = type;
        this.stacks = stacks;
        this.source = source;
        this.icon = icon;
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

    public int GetDamage()
    {
        if (source == null) return 0;

        switch (type)
        {
            case DebuffType.Burn:
                // Burn: 75% ATK (stack chỉ là duration)
                return Mathf.RoundToInt(source.GetATK() * 0.75f);
            
            case DebuffType.Poison:
                // Poison: 50% ATK (stack chỉ là duration)
                return Mathf.RoundToInt(source.GetATK() * 0.5f);
            
            default:
                return 0;
        }
    }

    public float GetAttackReduction()
    {
        // DecreaseAttack: Giảm 40% ATK (stack chỉ là duration)
        if (type == DebuffType.DecreaseAttack)
        {
            return 0.4f; // 40% cố định
        }
        return 0f;
    }
}
