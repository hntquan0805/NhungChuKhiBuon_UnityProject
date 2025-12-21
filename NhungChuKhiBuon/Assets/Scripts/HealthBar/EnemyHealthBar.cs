using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage; // kéo Image đỏ/green vào đây
    public EnemyCharacter enemy;

    [Header("Destroy Settings")]
    public bool destroyWithEnemy = true;
    public bool fadeOutBeforeDestroy = true; // Fade out trước khi destroy
    public float fadeOutDuration = 0.5f;

    private float maxHP;
    private bool isFading = false;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        if (enemy != null)
        {
            maxHP = enemy.GetMaxHP();
            UpdateBar();
        }

        // Thêm CanvasGroup để fade out
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && fadeOutBeforeDestroy)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Update()
    {
        if (isFading) return; // Đang fade thì không làm gì nữa

        // Kiểm tra enemy còn tồn tại không
        if (enemy == null || enemy.GetCurrentHP() <= 0 || enemy.IsDead())
        {
            if (destroyWithEnemy)
            {
                if (fadeOutBeforeDestroy && canvasGroup != null)
                {
                    StartCoroutine(FadeOutAndDestroy());
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            return;
        }

        UpdateBar();
    }

    private void UpdateBar()
    {
        if (fillImage != null && enemy != null)
        {
            Vector3 s = fillImage.transform.localScale;
            s.x = (float)enemy.GetCurrentHP() / (float)maxHP;
            fillImage.transform.localScale = s;
        }
    }

    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        isFading = true;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeOutDuration);
            canvasGroup.alpha = alpha;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}