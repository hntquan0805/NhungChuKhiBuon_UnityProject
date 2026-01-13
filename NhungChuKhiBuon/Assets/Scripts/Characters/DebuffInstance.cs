using UnityEngine;

[System.Serializable]
public class DebuffInstance
{
    public DebuffType type;
    public int stacks; // Số lượng stack
    public CharacterBase source; // THAY ĐỔI: Dùng CharacterBase thay vì PlayerCharacter
    public Sprite icon; // Icon của debuff

    // Constructor cho PlayerCharacter gây debuff lên Enemy
    public DebuffInstance(DebuffType type, int stacks, PlayerCharacter source, Sprite icon = null)
    {
        this.type = type;
        this.stacks = stacks;
        this.source = source;
        this.icon = icon;
    }

    // Constructor cho EnemyCharacter gây debuff lên Player (OVERLOAD MỚI)
    public DebuffInstance(DebuffType type, int stacks, EnemyCharacter source, Sprite icon = null)
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

        // Lấy ATK từ CharacterBase (có thể là Player hoặc Enemy)
        int sourceATK = 0;

        if (source is PlayerCharacter player)
        {
            sourceATK = player.GetATK();
        }
        else if (source is EnemyCharacter enemy)
        {
            sourceATK = enemy.GetATK();
        }

        switch (type)
        {
            case DebuffType.Burn:
                // Burn: 75% ATK (stack chỉ là duration)
                return Mathf.RoundToInt(sourceATK * 0.75f);

            case DebuffType.Poison:
                return Mathf.RoundToInt(sourceATK * 0.5f * stacks);

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