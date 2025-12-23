using UnityEngine;

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

    private int currentCP;
    private PlayerTeam targetTeam; // Đổi từ PlayerCharacter → PlayerTeam
    private bool isDead = false;

    protected override void Awake()
    {
        // Sử dụng maxHP từ stats
        maxHP = stats.maxHP;
        base.Awake();
        currentCP = maxCP;
    }

    public void InitializeCP(int min, int max)
    {
        maxCP = Random.Range(min, max + 1);
        currentCP = maxCP;
        Debug.Log($"{gameObject.name} initialized with CP: {currentCP}/{maxCP}");
    }

    public override void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log($"[ENEMY DAMAGE] {gameObject.name} took {amount} damage. HP: {currentHP}/{maxHP}");

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

        Debug.Log($"☠️ {gameObject.name} has been defeated!");

        PlayDeath();

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.enemies.Remove(this);
            Debug.Log($"Removed {gameObject.name} from enemy list. Remaining enemies: {BattleManager.Instance.enemies.Count}");
        }

        if (TargetSelector.Instance != null)
        {
            if (TargetSelector.Instance.GetCurrentSelectedEnemy() == this)
            {
                Debug.Log($"{gameObject.name} was selected target, finding new target...");
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
        yield return new WaitForSeconds(destroyDelay - fadeOutDuration);

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeOutDuration);

            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    Color color = sprite.color;
                    color.a = alpha;
                    sprite.color = color;
                }
            }

            yield return null;
        }

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
            Debug.LogError("[ENEMY ATTACK] targetTeam is NULL!");
            return;
        }

        // Tính damage dựa trên stats
        int baseDamage = Mathf.RoundToInt(stats.atk * stats.attackPercent / 100f);

        // Tính critical
        EnemyDamageResult damageResult = CalculateDamage(baseDamage);

        string critText = damageResult.isCritical ? " [CRITICAL HIT!]" : "";
        Debug.Log($"[ENEMY ATTACK] {gameObject.name} attacking TEAM with {damageResult.finalDamage} damage{critText} (Base: {baseDamage}, {stats.attackPercent}% ATK)");

        // Áp dụng defense của team
        int teamDefense = targetTeam.GetTotalDefense();
        int actualDamage = Mathf.RoundToInt(damageResult.finalDamage * (damageResult.finalDamage / (float)(damageResult.finalDamage + teamDefense)));
        Debug.Log($"[ENEMY ATTACK] Calculated damage before defense: {actualDamage}");
        actualDamage = Mathf.Max(actualDamage, 0);

        Debug.Log($"[ENEMY ATTACK] After team defense ({teamDefense}%): {actualDamage} damage");

        // Xử lý shield
        int totalShield = 0;
        foreach (var player in targetTeam.players)
        {
            totalShield += player.GetShieldAmount();
        }

        int remainingDamage = actualDamage;

        if (totalShield > 0)
        {
            int shieldToAbsorb = Mathf.Min(totalShield, actualDamage);
            remainingDamage -= shieldToAbsorb;

            Debug.Log($"[SHIELD] Absorbed {shieldToAbsorb} damage. Remaining: {remainingDamage}");

            foreach (var player in targetTeam.players)
            {
                int playerShield = player.GetShieldAmount();
                if (playerShield > 0)
                {
                    float ratio = (float)playerShield / totalShield;
                    int shieldLoss = Mathf.CeilToInt(shieldToAbsorb * ratio);
                    shieldLoss = Mathf.Min(shieldLoss, playerShield);

                    player.ReduceShield(shieldLoss);
                }
            }
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

                Debug.Log($"[TEAM DAMAGE] Splitting {remainingDamage} damage among {playersAlive} players = {damagePerPlayer} each");

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
            // Chỉ play hurt animation nếu có shield block hết
            foreach (var player in targetTeam.players)
            {
                player.PlayHurt();
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
        return stats.atk;
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
}

// Struct cho enemy damage result
[System.Serializable]
public struct EnemyDamageResult
{
    public int rawDamage;
    public int finalDamage;
    public bool isCritical;
}