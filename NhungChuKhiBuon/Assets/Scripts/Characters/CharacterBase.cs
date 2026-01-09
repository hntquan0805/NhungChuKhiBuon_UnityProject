using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHP = 100;
    protected int currentHP;

    [Header("Animator")]
    [SerializeField] protected Animator animator;

    protected virtual void Awake()
    {
        currentHP = maxHP;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public virtual void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        PlayHurt();

        if (currentHP <= 0)
            PlayDeath();
    }

    public virtual void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);

        PlayHeal();
    }

    public virtual void PlayHurt()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
    }

    protected virtual void PlayHeal()
    {
        if (animator != null)
            animator.SetTrigger("Heal");
    }

    protected virtual void PlayDeath()
    {
        if (animator != null)
            animator.SetTrigger("Die");
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
}
