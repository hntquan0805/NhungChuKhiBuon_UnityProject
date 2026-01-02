using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage; // Thanh máu (fill bar)
    public TextMeshProUGUI healthText; // Text hiển thị HP
    public EnemyCharacter enemy;

    [Header("Display Settings")]
    public string healthFormat = "{0}/{1}"; // Format: currentHP/maxHP
    public bool showText = true;
    public bool showOnlyWhenDamaged = false; // Chỉ hiện text khi bị damage

    [Header("Visual Settings")]
    public Color healthColor = new Color(0.8f, 0.2f, 0.2f); // Màu đỏ cho enemy
    public Color lowHealthColor = new Color(1f, 0.3f, 0f); // Màu cam khi HP thấp
    [Range(0f, 0.5f)]
    public float lowHealthThreshold = 0.25f; // Ngưỡng HP thấp (25%)

    [Header("Animation")]
    public bool animateFill = true;
    public float fillSpeed = 5f;

    [Header("Destroy Settings")]
    public bool destroyWithEnemy = true;
    public bool fadeOutBeforeDestroy = true;
    public float fadeOutDuration = 0.5f;

    private int maxHP;
    private float currentFillAmount = 1f;
    private bool isFading = false;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (enemy != null)
        {
            maxHP = enemy.GetMaxHP();
            UpdateBar();
        }

        // Set màu cho fill image
        if (fillImage != null)
        {
            fillImage.color = healthColor;
        }

        // Thêm CanvasGroup để fade out
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && fadeOutBeforeDestroy)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Ẩn text nếu cần
        if (healthText != null && showOnlyWhenDamaged)
        {
            healthText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isFading) return;

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
        if (enemy == null || maxHP <= 0)
            return;

        int currentHP = enemy.GetCurrentHP();
        float targetFillAmount = Mathf.Clamp01((float)currentHP / maxHP);

        // Animation smooth
        if (animateFill)
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * fillSpeed);
        }
        else
        {
            currentFillAmount = targetFillAmount;
        }

        // Update fill image
        if (fillImage != null)
        {
            Vector3 s = fillImage.transform.localScale;
            s.x = currentFillAmount;
            fillImage.transform.localScale = s;

            // Đổi màu khi HP thấp
            if (currentFillAmount <= lowHealthThreshold)
            {
                fillImage.color = Color.Lerp(fillImage.color, lowHealthColor, Time.deltaTime * fillSpeed);
            }
            else
            {
                fillImage.color = Color.Lerp(fillImage.color, healthColor, Time.deltaTime * fillSpeed);
            }
        }

        // Update text
        if (showText && healthText != null)
        {
            // Hiện text nếu showOnlyWhenDamaged = true và đã bị damage
            if (showOnlyWhenDamaged && currentHP < maxHP)
            {
                healthText.gameObject.SetActive(true);
            }

            healthText.text = string.Format(healthFormat, currentHP, maxHP);
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

    /// <summary>
    /// Force refresh bar ngay lập tức (không animation)
    /// </summary>
    public void RefreshImmediate()
    {
        if (enemy == null || maxHP <= 0)
            return;

        int currentHP = enemy.GetCurrentHP();
        currentFillAmount = (float)currentHP / maxHP;

        if (fillImage != null)
        {
            Vector3 s = fillImage.transform.localScale;
            s.x = currentFillAmount;
            fillImage.transform.localScale = s;
        }

        if (showText && healthText != null)
        {
            healthText.text = string.Format(healthFormat, currentHP, maxHP);
        }
    }

    /// <summary>
    /// Set health format tùy chỉnh
    /// </summary>
    public void SetHealthFormat(string format)
    {
        healthFormat = format;
    }

    /// <summary>
    /// Show/hide health text
    /// </summary>
    public void SetShowText(bool show)
    {
        showText = show;
        if (healthText != null)
        {
            healthText.gameObject.SetActive(show);
        }
    }
}