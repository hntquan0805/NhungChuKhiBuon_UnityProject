using UnityEngine;
using System.Collections.Generic;

public class PlayerCharacter : CharacterBase
{
    [Header("Player Stats")]
    public PlayerStats stats = new PlayerStats();

    private EnemyCharacter targetEnemy;
    
    // Buff system
    private List<BuffInstance> buffs = new List<BuffInstance>();

    private List<DebuffInstance> debuffs = new List<DebuffInstance>();

    [Header("Audio")]
    private AudioSource audioSource;

    // Death system
    protected bool isDead = false;
    [Header("Death Effect")]
    public float fadeOutDuration = 1.5f;

    protected override void Awake()
    {
        maxHP = stats.maxHP;
        base.Awake();
        
        // Thêm AudioSource component nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        
        // Phát audio attack
        PlaySound(stats.attackSound);
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

        // Phát audio shield
        PlaySound(stats.shieldSound);

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
        
        // Phát audio cast
        PlaySound(stats.castSound);
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

    public override void TakeDamage(int amount)
    {
        if (isDead) return;
        
        base.TakeDamage(amount);
        
        // Phát audio hurt
        PlaySound(stats.hurtSound);
        
        // Kiểm tra nếu chết
        if (currentHP <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(FadeOutAndDestroy());
        }
    }

    public override void Heal(int amount)
    {
        base.Heal(amount);
        
        // Phát audio heal
        PlaySound(stats.healSound);
    }

    public void HealSilent(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    /// <summary>
    /// Refresh max HP và các stats sau khi level up.
    /// </summary>
    public void RefreshStatsAfterLevelUp()
    {
        int newMaxHP = stats.maxHP;
        
        // Tăng current HP theo tỉ lệ với max HP mới
        float hpPercentage = (float)currentHP / maxHP;
        maxHP = newMaxHP;
        currentHP = Mathf.RoundToInt(maxHP * hpPercentage);
        currentHP = Mathf.Clamp(currentHP, 1, maxHP);
        
        Debug.Log($"[{name}] Stats refreshed after level up. New MaxHP: {maxHP}, CurrentHP: {currentHP}");
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
                // Heal toàn đội với amount dựa trên maxHP của caster (không trigger animation)
                int healAmount = buff.GetHealAmount();
                HealWholeTeamSilent(healAmount);
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
    
    private void HealWholeTeamSilent(int amount)
    {
        // Tìm PlayerTeam trong scene và heal toàn đội (không trigger animation)
        PlayerTeam team = FindObjectOfType<PlayerTeam>();
        if (team != null)
        {
            foreach (var player in team.players)
            {
                if (player != null && player.GetCurrentHP() > 0)
                {
                    player.HealSilent(amount);
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

            TeamDebuffManager debuffManager = team.GetComponent<TeamDebuffManager>();
            if (debuffManager != null)
            {
                debuffManager.UpdateDebuffUI();
            }
        }
    }
    
    public void ClearBuffs()
    {
        buffs.Clear();
        UpdateTeamBuffUI();
    }


    // ========== DEBUFF SYSTEM - SỬ DỤNG LẠI DebuffInstance ==========

    // Overload cho Enemy gây debuff
    public void AddDebuff(DebuffType type, int stacks, EnemyCharacter source, Sprite icon = null)
    {
        if (type == DebuffType.DecreaseAttack || type == DebuffType.Poison)
        {
            DebuffInstance existingDebuff = debuffs.Find(d => d.type == type);

            Debug.Log($"[AddDebuff] {name} - Type: {type} | Existing: {existingDebuff != null} | Current debuffs count: {debuffs.Count}");

            if (existingDebuff != null)
            {
                Debug.Log($"[AddDebuff] CỘNG STACK: {existingDebuff.stacks} + {stacks} = {existingDebuff.stacks + stacks}");
                existingDebuff.AddStacks(stacks);
            }
            else
            {
                Debug.Log($"[AddDebuff] TẠO MỚI debuff {type} với {stacks} stack");
                DebuffInstance newDebuff = new DebuffInstance(type, stacks, source, icon);
                debuffs.Add(newDebuff);
            }
        }

        UpdateTeamDebuffUI();
    }

    // Overload cho Player gây debuff (nếu cần)
    public void AddDebuff(DebuffType type, int stacks, PlayerCharacter source, Sprite icon = null)
    {
        if (type == DebuffType.DecreaseAttack)
        {
            DebuffInstance existingDebuff = debuffs.Find(d => d.type == type);

            if (existingDebuff != null)
            {
                existingDebuff.AddStacks(stacks);
            }
            else
            {
                DebuffInstance newDebuff = new DebuffInstance(type, stacks, source, icon);
                debuffs.Add(newDebuff);
            }
        }
        else
        {
            DebuffInstance newDebuff = new DebuffInstance(type, stacks, source, icon);
            debuffs.Add(newDebuff);
        }

        UpdateTeamDebuffUI();
    }

    public void RemoveDebuff(DebuffType type)
    {
        debuffs.RemoveAll(d => d.type == type);
        UpdateTeamDebuffUI();
    }

    public void ProcessDebuffsAtTurnStart()
    {
        int totalDamage = 0;
        List<DebuffInstance> debuffsToRemove = new List<DebuffInstance>();
        Debug.Log($"[{name}] ProcessDebuffs - Total debuffs: {debuffs.Count}");

        foreach (var debuff in debuffs)
        {
            int damage = debuff.GetDamage();
            Debug.Log($"[{name}] Debuff {debuff.type} - Damage: {damage}, Stacks: {debuff.stacks}");
            if (damage > 0)
            {
                totalDamage += damage;
            }

            // Giảm 1 stack
            debuff.ReduceStacks(1);

            // Nếu hết stack thì đánh dấu để xóa
            if (debuff.stacks <= 0)
            {
                debuffsToRemove.Add(debuff);
            }
        }

        // Xóa các debuff đã hết stack
        foreach (var debuff in debuffsToRemove)
        {
            debuffs.Remove(debuff);
        }
        Debug.Log($"[{name}] Total damage from debuffs: {totalDamage}");
        // Gây damage
        if (totalDamage > 0)
        {
            TakeDamage(totalDamage);
        }

        // Update UI
        UpdateTeamDebuffUI();
    }

    public List<DebuffInstance> GetDebuffs()
    {
        return debuffs;
    }

    private void UpdateTeamDebuffUI()
    {
        // Gọi team debuff manager để update UI
        PlayerTeam team = FindObjectOfType<PlayerTeam>();
        if (team != null)
        {
            TeamDebuffManager debuffManager = team.GetComponent<TeamDebuffManager>();
            if (debuffManager != null)
            {
                debuffManager.UpdateDebuffUI();
            }
        }
    }

    public void ClearDebuffs()
    {
        debuffs.Clear();
        UpdateTeamDebuffUI();
    }


    // ========== AUDIO SYSTEM ==========

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // ========== DEATH SYSTEM ==========
    
    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        // Lấy tất cả SpriteRenderer components
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        CanvasGroup canvasGroup = GetComponentInChildren<CanvasGroup>();
        
        float elapsed = 0f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeOutDuration);
            
            // Fade out tất cả sprites
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }
            }
            
            // Fade out canvas nếu có (healthbar, etc)
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            
            yield return null;
        }
        
        // Sau khi fade out xong, disable GameObject thay vì destroy
        gameObject.SetActive(false);
    }
    
    public bool IsDead()
    {
        return isDead;
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