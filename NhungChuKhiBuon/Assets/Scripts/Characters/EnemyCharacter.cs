using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class EnemyCharacter : CharacterBase
{
    [Header("Enemy Stats")]
    public EnemyStats stats = new EnemyStats();

    [Header("CP Settings")]
    [SerializeField] private int maxCP = 3;

    [Header("Death Settings")]
    [SerializeField] private float destroyDelay = 1.5f;
    [SerializeField] private bool fadeOutBeforeDestroy = false;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private TextMeshProUGUI cpText;

    [Header("Debuff UI")]
    [SerializeField] private Transform debuffIconContainer;
    [SerializeField] private GameObject debuffIconPrefab;

    [Header("Buff UI")]
    [SerializeField] private Transform buffIconContainer;
    [SerializeField] private GameObject buffIconPrefab;

    [Header("Passive Ability")]
    [SerializeField] private Sprite increaseAttackIcon;
    [SerializeField] private int passiveTurnInterval = 4;
    [SerializeField] private int passiveBuffStacks = 2;

    private int currentCP;
    protected PlayerTeam targetTeam; // Đổi từ PlayerCharacter → PlayerTeam (protected để subclass truy cập)
    private bool isDead = false;
    private int turnCounter = 0;
    
    // Debuff system
    private List<DebuffInstance> debuffs = new List<DebuffInstance>();
    private List<GameObject> debuffIcons = new List<GameObject>();
    
    // Buff system
    private List<BuffInstance> buffs = new List<BuffInstance>();
    private List<GameObject> buffIcons = new List<GameObject>();

    protected override void Awake()
    {
        // Sử dụng maxHP từ stats
        maxHP = stats.maxHP;
        base.Awake();
        currentCP = maxCP;
    }

    private void Start()
    {
        // Apply passive buff khi vào trận (có thể override)
        ApplyStartPassive();
    }

    public void InitializeCP(int min, int max)
    {
        maxCP = Random.Range(min, max + 1);
        currentCP = maxCP;
    }

    public override void TakeDamage(int amount)
    {
        if (isDead) return;

        // Kiểm tra Stealth buff
        BuffInstance stealthBuff = buffs.Find(b => b.type == BuffType.Stealth);
        if (stealthBuff != null)
        {
            // Giảm 20% sát thương nhận
            amount = Mathf.RoundToInt(amount * 0.8f);
            
            // Xóa Stealth sau khi bị tấn công
            buffs.Remove(stealthBuff);
            UpdateBuffUI();
            
            // Gọi OnStealthLost để subclass có thể override
            OnStealthLost();
        }

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        PlayHurt();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // Xóa tất cả debuff icons trước khi chết
        ClearDebuffIcons();

        PlayDeath();

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.enemies.Remove(this);
        }

        if (TargetSelector.Instance != null)
        {
            if (TargetSelector.Instance.GetCurrentSelectedEnemy() == this)
            {
            }
        }

        if (fadeOutBeforeDestroy)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        // Fade ngay lập tức, không đợi
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeOutDuration);

            // Fade sprites
            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    Color color = sprite.color;
                    color.a = alpha;
                    sprite.color = color;
                }
            }

            // Fade CP Text nếu có
            if (cpText != null)
            {
                Color textColor = cpText.color;
                textColor.a = alpha;
                cpText.color = textColor;
            }

            yield return null;
        }

        // Đợi thêm thời gian trước khi destroy
        yield return new WaitForSeconds(destroyDelay - fadeOutDuration);
        Destroy(gameObject);
    }

    // Đổi thành set target = PlayerTeam
    public void SetTarget(PlayerTeam team)
    {
        targetTeam = team;
    }

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
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
    }

    // Tính damage với crit (giống player)
    public EnemyDamageResult CalculateDamage(int baseDamage)
    {
        EnemyDamageResult result = new EnemyDamageResult();

        result.rawDamage = baseDamage;

        // Check critical hit
        result.isCritical = Random.Range(0, 100) < stats.crit;

        if (result.isCritical)
        {
            result.finalDamage = Mathf.RoundToInt(result.rawDamage * stats.critDam / 100f);
        }
        else
        {
            result.finalDamage = result.rawDamage;
        }

        return result;
    }

    // CP Management
    public int GetCurrentCP()
    {
        return currentCP;
    }

    public int GetMaxCP()
    {
        return maxCP;
    }

    public void SetCurrentCP(int value)
    {
        currentCP = Mathf.Clamp(value, 0, maxCP);
    }

    public void ReduceCP(int amount)
    {
        currentCP -= amount;
        currentCP = Mathf.Max(currentCP, 0);
    }

    public void ResetCP()
    {
        currentCP = maxCP;
    }

    public bool HasCPRemaining()
    {
        return currentCP > 0;
    }

    public int GetMaxHP()
    {
        return maxHP;
    }

    public bool IsDead()
    {
        return isDead;
    }

    // Getters cho stats
    public int GetATK()
    {
        int baseATK = stats.atk;
        
        // Tính tổng tăng ATK từ buff
        float totalBonus = 0f;
        foreach (var buff in buffs)
        {
            totalBonus += buff.GetAttackBonus();
        }
        
        // Tính tổng giảm ATK từ debuff
        float totalReduction = 0f;
        foreach (var debuff in debuffs)
        {
            totalReduction += debuff.GetAttackReduction();
        }
        
        // Giới hạn giảm tối đa 80% (không giảm quá nhiều)
        totalReduction = Mathf.Min(totalReduction, 0.8f);
        
        // Áp dụng cả buff và debuff
        int finalATK = Mathf.RoundToInt(baseATK * (1f + totalBonus - totalReduction));
        
        return finalATK;
    }

    public int GetDefense()
    {
        return stats.def;
    }

    public int GetCrit()
    {
        return stats.crit;
    }

    public int GetCritDam()
    {
        return stats.critDam;
    }

    // ========== DEBUFF SYSTEM ==========
    
    public void AddDebuff(DebuffType type, int stacks, PlayerCharacter source, Sprite icon = null)
    {
        // DecreaseAttack: Cộng stack nếu đã có
        // Burn và các debuff khác: Tạo mới
        if (type == DebuffType.DecreaseAttack)
        {
            DebuffInstance existingDebuff = debuffs.Find(d => d.type == type);
            
            if (existingDebuff != null)
            {
                // Đã có debuff này -> cộng thêm stack
                existingDebuff.AddStacks(stacks);
            }
            else
            {
                // Chưa có -> tạo mới
                DebuffInstance newDebuff = new DebuffInstance(type, stacks, source, icon);
                debuffs.Add(newDebuff);
            }
        }
        else
        {
            // Burn và các debuff khác: Luôn tạo mới
            DebuffInstance newDebuff = new DebuffInstance(type, stacks, source, icon);
            debuffs.Add(newDebuff);
        }
        
        UpdateDebuffUI();
    }
    
    public void RemoveDebuff(DebuffType type)
    {
        debuffs.RemoveAll(d => d.type == type);
        UpdateDebuffUI();
    }
    
    public void ProcessDebuffsAtTurnStart()
    {
        // Tăng turn counter và check passive ability (có thể override)
        turnCounter++;
        if (turnCounter >= passiveTurnInterval)
        {
            ApplyTurnPassive();
            turnCounter = 0;
        }
        
        // Process buff duration
        ProcessBuffs();
        
        int totalDamage = 0;
        List<DebuffInstance> debuffsToRemove = new List<DebuffInstance>();
        
        foreach (var debuff in debuffs)
        {
            int damage = debuff.GetDamage();
            if (damage > 0)
            {
                totalDamage += damage;
            }
            
            // Giảm 1 stack cho TẤT CẢ debuff
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
        
        // Gây damage
        if (totalDamage > 0)
        {
            TakeDamage(totalDamage);
        }
        
        // Update UI sau khi xử lý debuff
        UpdateDebuffUI();
    }
    
    public List<DebuffInstance> GetDebuffs()
    {
        return debuffs;
    }
    
    private void UpdateDebuffUI()
    {
        ClearDebuffIcons();
        
        if (debuffIconContainer == null || debuffIconPrefab == null)
        {
            return;
        }
        
        // Tạo icon mới cho mỗi debuff
        foreach (var debuff in debuffs)
        {
            GameObject iconObj = Instantiate(debuffIconPrefab, debuffIconContainer);
            
            // Đảm bảo icon là con của container
            iconObj.transform.SetParent(debuffIconContainer, false);
            
            // Reset scale và position local
            RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.localPosition = Vector3.zero;
            }
            
            DebuffIcon icon = iconObj.GetComponent<DebuffIcon>();
            if (icon != null)
            {
                icon.Initialize(debuff);
            }
            
            debuffIcons.Add(iconObj);
        }
    }
    
    private void ClearDebuffIcons()
    {
        foreach (var icon in debuffIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        debuffIcons.Clear();
    }
    
    // Buff system methods
    // Virtual method - Apply passive khi bắt đầu battle
    protected virtual void ApplyStartPassive()
    {
        if (increaseAttackIcon != null)
        {
            AddBuff(BuffType.IncreaseAttack, passiveBuffStacks, increaseAttackIcon);
        }
    }
    
    protected virtual void ApplyTurnPassive()
    {
        if (increaseAttackIcon != null)
        {
            AddBuff(BuffType.IncreaseAttack, passiveBuffStacks, increaseAttackIcon);
        }
    }
    
    public void AddBuff(BuffType type, int stacks, Sprite icon)
    {
        BuffInstance existingBuff = buffs.Find(b => b.type == type);
        
        if (existingBuff != null)
        {
            existingBuff.AddStacks(stacks);
        }
        else
        {
            BuffInstance newBuff = new BuffInstance(type, stacks, icon);
            buffs.Add(newBuff);
        }
        
        UpdateBuffUI();
    }
    
    public bool RemoveRandomBuff()
    {
        if (buffs.Count == 0)
            return false;
        
        int randomIndex = Random.Range(0, buffs.Count);
        buffs.RemoveAt(randomIndex);
        
        UpdateBuffUI();
        return true;
    }
    
    public List<BuffInstance> GetBuffs()
    {
        return buffs;
    }
    
    private void ProcessBuffs()
    {
        List<BuffInstance> buffsToRemove = new List<BuffInstance>();
        
        foreach (var buff in buffs)
        {
            buff.ReduceStacks(1);
            
            if (buff.stacks <= 0)
            {
                buffsToRemove.Add(buff);
            }
        }
        
        foreach (var buff in buffsToRemove)
        {
            buffs.Remove(buff);
        }
        
        // LUÔN update UI sau khi process buffs
        if (buffs.Count > 0 || buffsToRemove.Count > 0)
        {
            UpdateBuffUI();
        }
    }
    
    private void UpdateBuffUI()
    {
        ClearBuffIcons();
        
        if (buffIconContainer == null || buffIconPrefab == null)
            return;
        
        foreach (var buff in buffs)
        {
            GameObject iconObj = Instantiate(buffIconPrefab, buffIconContainer);
            iconObj.transform.SetParent(buffIconContainer, false);
            
            RectTransform rectTransform = iconObj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.localPosition = Vector3.zero;
            }
            
            BuffIcon icon = iconObj.GetComponent<BuffIcon>();
            if (icon != null)
                icon.Initialize(buff);
            
            buffIcons.Add(iconObj);
        }
    }
    
    private void ClearBuffIcons()
    {
        foreach (var icon in buffIcons)
        {
            if (icon != null)
                Destroy(icon);
        }
        buffIcons.Clear();
    }
    
    // Virtual method để subclass có thể override khi mất Stealth
    protected virtual void OnStealthLost()
    {
        // Override trong BossEnemy để reset turn counter
    }
    
    // Kiểm tra xem enemy có buff Stealth không
    public bool HasStealth()
    {
        return buffs.Exists(b => b.type == BuffType.Stealth);
    }
}

// Struct cho enemy damage result
[System.Serializable]
public struct EnemyDamageResult
{
    public int rawDamage;
    public int finalDamage;
    public bool isCritical;
}