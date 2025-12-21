using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    [Header("Enemy Stats")]
    [SerializeField] private int attackDamage = 8;
    [SerializeField] private int maxCP = 3;

    [Header("Death Settings")]
    [SerializeField] private float destroyDelay = 1.5f; // Thời gian đợi trước khi destroy (để animation chạy)
    [SerializeField] private bool fadeOutBeforeDestroy = false; // Có fade out không
    [SerializeField] private float fadeOutDuration = 0.5f;

    private int currentCP;
    private PlayerCharacter targetPlayer;
    private bool isDead = false;

    protected override void Awake()
    {
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
        if (isDead) return; // Không nhận damage nếu đã chết

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
        if (isDead) return; // Tránh gọi Die() nhiều lần

        isDead = true;

        Debug.Log($"☠️ {gameObject.name} has been defeated!");

        // Trigger animation chết
        PlayDeath();

        // Xóa khỏi danh sách enemies trong BattleManager
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.enemies.Remove(this);
            Debug.Log($"Removed {gameObject.name} from enemy list. Remaining enemies: {BattleManager.Instance.enemies.Count}");
        }

        // Nếu enemy này đang là target, clear target
        if (TargetSelector.Instance != null)
        {
            if (TargetSelector.Instance.GetCurrentSelectedEnemy() == this)
            {
                Debug.Log($"{gameObject.name} was selected target, finding new target...");
                // TargetSelector sẽ tự động tìm target mới trong GetCurrentSelectedEnemy()
            }
        }

        // Destroy sau một khoảng delay để animation chạy
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
        // Đợi animation chết chạy một chút
        yield return new WaitForSeconds(destroyDelay - fadeOutDuration);

        // Fade out
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

    public void SetTarget(PlayerCharacter player)
    {
        targetPlayer = player;
    }

    public void PlayAttack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");
    }

    public void DealDamage()
    {
        if (targetPlayer == null) return;

        PlayerTeam team = targetPlayer.GetComponentInParent<PlayerTeam>();

        int teamDefense = team.GetTotalDefense();
        int actualDamage = attackDamage - Mathf.RoundToInt(attackDamage * teamDefense / 100f);
        actualDamage = Mathf.Max(actualDamage, 0);

        int totalShield = 0;
        foreach (var player in team.players)
        {
            totalShield += player.GetShieldAmount();
        }

        int remainingDamage = actualDamage;

        if (totalShield > 0)
        {
            int shieldToAbsorb = Mathf.Min(totalShield, actualDamage);
            remainingDamage -= shieldToAbsorb;

            foreach (var player in team.players)
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

        if (remainingDamage > 0)
        {
            int playersAlive = 0;
            foreach (var player in team.players)
            {
                if (player.GetCurrentHP() > 0)
                    playersAlive++;
            }

            if (playersAlive > 0)
            {
                int damagePerPlayer = Mathf.CeilToInt((float)remainingDamage / playersAlive);

                foreach (var player in team.players)
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
            foreach (var player in team.players)
            {
                player.PlayHurt();
            }
        }
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
}