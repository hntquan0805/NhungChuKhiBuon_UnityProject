using UnityEngine;

public class ArenaSceneInitializer : MonoBehaviour
{
    [Header("Arena Settings")]
    [Range(0f, 1f)]
    [Tooltip("Percentage of HP to reduce (0.3 = 30%)")]
    public float hpReductionPercent = 0.3f;

    [Header("References")]
    public PlayerTeam playerTeam;

    [Header("Visual Feedback")]
    public bool showDebugLog = true;
    public bool playHurtAnimation = true;

    private void Start()
    {
        // Đợi 1 frame để đảm bảo PlayerTeam đã được khởi tạo
        Invoke("ApplyArenaDebuff", 0.1f);
    }

    private void ApplyArenaDebuff()
    {
        if (playerTeam == null)
        {
            Debug.LogError("[Arena] PlayerTeam reference is missing!");
            return;
        }

        if (playerTeam.players == null || playerTeam.players.Count == 0)
        {
            Debug.LogError("[Arena] No players found in PlayerTeam!");
            return;
        }

        if (showDebugLog)
        {
            Debug.Log($"⚔️ [Arena] Applying {hpReductionPercent * 100}% HP reduction to team...");
        }

        // Áp dụng debuff cho từng player
        foreach (var player in playerTeam.players)
        {
            if (player == null) continue;

            int currentHP = player.GetCurrentHP();
            int maxHP = player.GetMaxHP();

            // Tính damage = 30% max HP
            int damage = Mathf.RoundToInt(maxHP * hpReductionPercent);

            // Đảm bảo không chết (tối thiểu còn 1 HP)
            damage = Mathf.Min(damage, currentHP - 1);

            if (damage > 0)
            {
                // Giảm HP trực tiếp (không trigger hurt animation qua TakeDamage)
                player.TakeDamage(damage);

                if (showDebugLog)
                {
                    Debug.Log($"[Arena] {player.gameObject.name}: {currentHP} → {player.GetCurrentHP()} HP (-{damage})");
                }

                // Play hurt animation nếu muốn
                if (playHurtAnimation)
                {
                    player.PlayHurt();
                }
            }
        }

        if (showDebugLog)
        {
            int totalHP = playerTeam.GetTotalCurrentHP();
            int totalMaxHP = playerTeam.GetTotalMaxHP();
            Debug.Log($"⚔️ [Arena] Team HP: {totalHP}/{totalMaxHP}");
        }
    }

    // Optional: Method để apply custom reduction percent từ code khác
    public void ApplyCustomDebuff(float customPercent)
    {
        hpReductionPercent = Mathf.Clamp01(customPercent);
        ApplyArenaDebuff();
    }

    // Optional: Method để restore HP sau khi hoàn thành Arena
    public void RestoreTeamHP()
    {
        if (playerTeam == null || playerTeam.players == null) return;

        foreach (var player in playerTeam.players)
        {
            if (player == null) continue;

            int maxHP = player.GetMaxHP();
            int currentHP = player.GetCurrentHP();
            int healAmount = maxHP - currentHP;

            if (healAmount > 0)
            {
                player.HealSilent(healAmount);

                if (showDebugLog)
                {
                    Debug.Log($"[Arena] Restored {player.gameObject.name} to full HP: {player.GetCurrentHP()}/{maxHP}");
                }
            }
        }
    }
}