using UnityEngine;

public class MinionEnemy : EnemyCharacter
{
    [Header("Minion Settings")]
    [SerializeField] private int passiveInterval = 2; // Sau mỗi 2 turn
    [SerializeField] private float defenseIncreasePercent = 10f; // Tăng 10% DEF
    
    private int defenseTurnCounter = 0;
    private float currentDefenseMultiplier = 1f; // Multiplier cho defense
    
    protected override void Awake()
    {
        base.Awake();
        defenseTurnCounter = 0;
        currentDefenseMultiplier = 1f;
        
        // Minion không có CP vì không tấn công
        SetCurrentCP(0);
    }
    
    // Override để minion không tăng attack từ passive của base class
    protected override void ApplyStartPassive()
    {
        // Minion không có passive tăng attack
    }
    
    protected override void ApplyTurnPassive()
    {
        // Minion không có passive tăng attack
    }
    
    // Override InitializeCP để luôn giữ CP = 0
    public new void InitializeCP(int min, int max)
    {
        SetCurrentCP(0);
    }
    
    // Override ProcessDebuffsAtTurnStart để thêm passive
    public new void ProcessDebuffsAtTurnStart()
    {
        // Gọi base để xử lý debuff và buff
        base.ProcessDebuffsAtTurnStart();
        
        // Tăng turn counter
        defenseTurnCounter++;
        
        // Sau mỗi 2 turn, tăng defense
        if (defenseTurnCounter >= passiveInterval)
        {
            IncreaseDefense();
            defenseTurnCounter = 0; // Reset counter
        }
    }
    
    private void IncreaseDefense()
    {
        currentDefenseMultiplier += (defenseIncreasePercent / 100f);
    }
    
    public new int GetDefense()
    {
        int baseDef = stats.def;
        return Mathf.RoundToInt(baseDef * currentDefenseMultiplier);
    }
    
    public new void DealDamage()
    {
        // Minion không tấn công
    }
    
    // Override PlayAttack để không hiển thị animation tấn công
    public new void PlayAttack()
    {
        // Minion không tấn công, không play animation
    }
}
