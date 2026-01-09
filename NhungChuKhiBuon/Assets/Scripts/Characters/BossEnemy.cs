using UnityEngine;
using System.Collections.Generic;

public class BossEnemy : EnemyCharacter
{
    [Header("Boss Settings")]
    [SerializeField] private Sprite stealthIcon;
    [SerializeField] private GameObject minionPrefab1; // Enemy minion type 1
    [SerializeField] private GameObject minionPrefab2; // Enemy minion type 2
    [SerializeField] private Transform spawnPoint1; // Vị trí spawn minion 1
    [SerializeField] private Transform spawnPoint2; // Vị trí spawn minion 2
    
    [Header("Boss Passive Settings")]
    [SerializeField] private int passiveInterval = 3; // Sau 3 turn kể từ khi mất Stealth
    [SerializeField] private int stealthDuration = 2; // Stealth kéo dài 2 turn
    [SerializeField] private int passiveAttackPercent = 125; // 125% ATK
    
    private int turnsSinceStealthLost = 0;
    
    protected override void Awake()
    {
        base.Awake();
        turnsSinceStealthLost = 0;
    }
    

    
    // Override để Boss không có passive tăng attack từ base class
    protected override void ApplyStartPassive()
    {
        // Boss không tự tăng attack khi bắt đầu
    }
    
    protected override void ApplyTurnPassive()
    {
        // Boss không tự tăng attack mỗi turn
    }
    
    // Override ProcessDebuffsAtTurnStart để thêm logic passive của boss
    public new void ProcessDebuffsAtTurnStart()
    {
        // Gọi base để xử lý debuff và buff bình thường
        base.ProcessDebuffsAtTurnStart();
        
        // Kiểm tra xem có Stealth không
        if (!HasStealth())
        {
            // Tăng counter nếu không có Stealth
            turnsSinceStealthLost++;
            
            // Kích hoạt passive sau mỗi 4 turn
            if (turnsSinceStealthLost >= passiveInterval)
            {
                ActivatePassive();
                turnsSinceStealthLost = 0; // Reset counter
            }
        }
    }
    
    protected override void OnStealthLost()
    {
        // Reset counter khi mất Stealth
        turnsSinceStealthLost = 0;
    }
    
    private void ActivatePassive()
    {
        AttackWithPassive();
        ApplySelfStealth();
        SummonMinions();
    }
    
    private void AttackWithPassive()
    {
        if (targetTeam == null) return;
        
        int originalAttackPercent = stats.attackPercent;
        stats.attackPercent = passiveAttackPercent;
        
        PlayAttack();
        DealDamage();
        
        stats.attackPercent = originalAttackPercent;
    }
    
    private void ApplySelfStealth()
    {
        if (stealthIcon != null)
        {
            AddBuff(BuffType.Stealth, stealthDuration, stealthIcon);
        }
    }
    
    private void SummonMinions()
    {
        if (BattleManager.Instance == null) return;
        
        MinionEnemy summonedMinion = null;
        
        // Triệu hồi minion 1
        if (minionPrefab1 != null && spawnPoint1 != null)
        {
            GameObject minion1 = Instantiate(minionPrefab1, spawnPoint1.position, Quaternion.identity);
            
            if (spawnPoint1.parent != null)
            {
                minion1.transform.SetParent(spawnPoint1.parent);
                minion1.transform.position = spawnPoint1.position;
            }
            
            MinionEnemy minionEnemy1 = minion1.GetComponentInChildren<MinionEnemy>();
            if (minionEnemy1 != null)
            {
                BattleManager.Instance.enemies.Add(minionEnemy1);
                minionEnemy1.SetTarget(BattleManager.Instance.playerTeam);
                
                if (summonedMinion == null)
                {
                    summonedMinion = minionEnemy1;
                }
            }
        }
        
        // Triệu hồi minion 2
        if (minionPrefab2 != null && spawnPoint2 != null)
        {
            GameObject minion2 = Instantiate(minionPrefab2, spawnPoint2.position, Quaternion.identity);
            
            if (spawnPoint2.parent != null)
            {
                minion2.transform.SetParent(spawnPoint2.parent);
                minion2.transform.position = spawnPoint2.position;
            }
            
            MinionEnemy minionEnemy2 = minion2.GetComponentInChildren<MinionEnemy>();
            if (minionEnemy2 != null)
            {
                BattleManager.Instance.enemies.Add(minionEnemy2);
                minionEnemy2.SetTarget(BattleManager.Instance.playerTeam);
                
                if (summonedMinion == null)
                {
                    summonedMinion = minionEnemy2;
                }
            }
        }
        
        // Nếu target đang là boss thì chuyển sang minion vừa spawn
        if (summonedMinion != null && TargetSelector.Instance != null)
        {
            EnemyCharacter currentTarget = TargetSelector.Instance.GetCurrentSelectedEnemy();
            if (currentTarget == this)
            {
                TargetSelector.Instance.SelectEnemy(summonedMinion);
            }
        }
    }
    
    // Override skill thường: Khi hết CP sẽ tấn công 75% ATK
    public void PerformNormalSkill()
    {
        if (GetCurrentCP() > 0 || targetTeam == null) return;
        
        int originalAttackPercent = stats.attackPercent;
        stats.attackPercent = 75;
        
        PlayAttack();
        DealDamage();
        
        stats.attackPercent = originalAttackPercent;
    }
}
