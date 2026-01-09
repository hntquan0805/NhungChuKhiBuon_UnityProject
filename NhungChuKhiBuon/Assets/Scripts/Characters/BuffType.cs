using UnityEngine;

public enum BuffType
{
    IncreaseAttack,
    IncreaseDefense,
    IncreaseSpeed,
    Regeneration,
    ContinuousHeal,
    Stealth // Không thể bị chỉ định, giảm 20% sát thương nhận, mất khi bị tấn công
    // Có thể thêm các buff khác sau
}
