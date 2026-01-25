using UnityEngine;

public class GoblinEnemy : EnemyCharacter
{
    [Header("Goblin Passive Settings")]
    [SerializeField] private float reviveHpPercent = 75f; // Hồi 75% MaxHP

    private bool hasRevived = false; // Đánh dấu đã hồi sinh chưa

    protected override void Awake()
    {
        base.Awake();
        hasRevived = false;
    }

    // Tắt passive tăng attack từ base class
    protected override void ApplyStartPassive()
    {
        // Goblin không có passive tăng attack khi bắt đầu
    }

    protected override void ApplyTurnPassive()
    {
        // Goblin không có passive tăng attack mỗi turn
    }

    // Override TakeDamage để xử lý passive hồi sinh
    public override void TakeDamage(int amount)
    {
        // Sử dụng IsDead() thay vì isDead
        if (IsDead()) return;

        // Kiểm tra Stealth buff
        BuffInstance stealthBuff = GetBuffs().Find(b => b.type == BuffType.Stealth);
        if (stealthBuff != null)
        {
            // Giảm 20% sát thương nhận
            amount = Mathf.RoundToInt(amount * 0.8f);

            // Xóa Stealth sau khi bị tấn công
            GetBuffs().Remove(stealthBuff);

            OnStealthLost();
        }

        currentHP -= amount;

        // ===== PASSIVE: HỒI SINH KHI LẦN ĐẦU HP <= 0 =====
        // Passive này CHẮC CHẮN kích hoạt, không thể bị hủy bỏ
        if (currentHP <= 0 && !hasRevived)
        {
            Debug.Log($"[GoblinEnemy] PASSIVE REVIVE TRIGGERED!");

            // Hồi sinh với 75% MaxHP
            int reviveAmount = Mathf.RoundToInt(GetMaxHP() * reviveHpPercent / 100f);
            currentHP = reviveAmount;

            // Đánh dấu đã hồi sinh (chỉ trigger 1 lần duy nhất)
            hasRevived = true;

            // Play animation hồi phục
            PlayHeal();

            Debug.Log($"[GoblinEnemy] Revived! HP: {currentHP}/{GetMaxHP()}");

            return; // KHÔNG CHẾT, return ngay
        }

        // Nếu HP <= 0 và đã hồi sinh rồi -> CHẾT THẬT
        if (currentHP <= 0)
        {
            currentHP = 0;
            PlayHurt();
            PlayDeath();

            Debug.Log($"[GoblinEnemy] Dead for real (already revived once)");

            // Xử lý chết
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.enemies.Remove(this);
            }

            if (TargetSelector.Instance != null)
            {
                if (TargetSelector.Instance.GetCurrentSelectedEnemy() == this)
                {
                    // Target selector sẽ tự động chọn enemy khác
                }
            }

            Destroy(gameObject, 1.5f);
        }
        else
        {
            // Bị damage nhưng chưa chết
            PlayHurt();
        }
    }
}