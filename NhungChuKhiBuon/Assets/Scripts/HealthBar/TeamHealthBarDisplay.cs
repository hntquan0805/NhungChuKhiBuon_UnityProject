using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Hiển thị HP và Shield của team
public class TeamHealthDisplay : MonoBehaviour
{
    [Header("HP Bar")]
    public Image healthFillImage;
    public TextMeshProUGUI healthText;

    [Header("Shield Bar (Overlay)")]
    public Image shieldFillImage;      // Shield đè lên HP, fill từ trái
    public TextMeshProUGUI shieldText; // Text hiển thị số shield
    public bool hideShieldWhenZero = true;

    [Header("Team Reference")]
    public PlayerTeam team;

    [Header("Display Settings")]
    public string healthFormat = "{0}/{1}";
    public string shieldFormat = "+{0}";
    public bool showHealthText = true;
    public bool showShieldText = true;

    [Header("Shield Calculation")]
    public bool usePercentageOfMaxHP = true; // Shield tính theo % max HP
    public int fixedMaxShield = 100; // Nếu không dùng %, dùng giá trị cố định này

    [Header("Visual Settings")]
    public Color healthColor = new Color(0.2f, 0.8f, 0.2f); // Xanh lá
    public Color shieldColor = new Color(0.5f, 0.8f, 1f);   // Xanh dương
    [Range(0.3f, 1f)]
    public float shieldAlpha = 0.75f; // Độ trong suốt của shield (để thấy HP phía sau)

    [Header("Animation")]
    public bool animateFill = true;
    public float fillSpeed = 5f;

    private float currentHealthFill = 1f;
    private float currentShieldFill = 0f;
    private CanvasGroup shieldCanvasGroup;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        // Set màu cho HP bar
        if (healthFillImage != null)
        {
            healthFillImage.color = healthColor;
        }

        // Set màu cho Shield bar (với alpha để thấy HP phía sau)
        if (shieldFillImage != null)
        {
            Color shieldCol = shieldColor;
            shieldCol.a = shieldAlpha;
            shieldFillImage.color = shieldCol;

            // Thêm CanvasGroup để fade in/out
            shieldCanvasGroup = shieldFillImage.GetComponent<CanvasGroup>();
            if (shieldCanvasGroup == null)
            {
                shieldCanvasGroup = shieldFillImage.gameObject.AddComponent<CanvasGroup>();
            }

            // Bắt đầu với alpha = 0
            shieldCanvasGroup.alpha = 0f;
        }

        // Ẩn shield text ban đầu
        if (shieldText != null)
        {
            shieldText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (team == null)
            return;

        UpdateHealthBar();
        UpdateShieldBar();
    }

    private void UpdateHealthBar()
    {
        int maxHP = team.GetTotalMaxHP();
        int currentHP = team.GetTotalCurrentHP();

        if (maxHP <= 0)
            return;

        // Tính tỉ lệ HP
        float targetHealthFill = Mathf.Clamp01((float)currentHP / maxHP);

        // Animation smooth
        if (animateFill)
        {
            currentHealthFill = Mathf.Lerp(currentHealthFill, targetHealthFill, Time.deltaTime * fillSpeed);
        }
        else
        {
            currentHealthFill = targetHealthFill;
        }

        // Update HP fill image
        if (healthFillImage != null)
        {
            Vector3 s = healthFillImage.transform.localScale;
            s.x = currentHealthFill;
            healthFillImage.transform.localScale = s;
        }

        // Update HP text
        if (showHealthText && healthText != null)
        {
            healthText.text = string.Format(healthFormat, currentHP, maxHP);
        }
    }

    private void UpdateShieldBar()
    {
        int shield = team.GetTeamShield();

        if (hideShieldWhenZero && shield <= 0)
        {
            // Fade out
            if (shieldCanvasGroup != null && shieldCanvasGroup.alpha > 0)
            {
                shieldCanvasGroup.alpha = Mathf.Lerp(shieldCanvasGroup.alpha, 0, Time.deltaTime * fillSpeed);
            }

            // Ẩn text
            if (shieldText != null && shieldText.gameObject.activeSelf)
            {
                shieldText.gameObject.SetActive(false);
            }

            return;
        }

        // Shield > 0: Hiện bar
        if (shieldCanvasGroup != null && shieldCanvasGroup.alpha < 1)
        {
            shieldCanvasGroup.alpha = Mathf.Lerp(shieldCanvasGroup.alpha, 1, Time.deltaTime * fillSpeed);
        }
        float targetShieldFill;

        if (usePercentageOfMaxHP)
        {
            // Tính shield dựa trên % của max HP
            int maxHP = team.GetTotalMaxHP();
            if (maxHP > 0)
            {
                targetShieldFill = Mathf.Clamp01((float)shield / maxHP);
            }
            else
            {
                targetShieldFill = 0f;
            }
        }
        else
        {
            targetShieldFill = Mathf.Clamp01((float)shield / fixedMaxShield);
        }

        // Animation smooth
        if (animateFill)
        {
            currentShieldFill = Mathf.Lerp(currentShieldFill, targetShieldFill, Time.deltaTime * fillSpeed);
        }
        else
        {
            currentShieldFill = targetShieldFill;
        }

        if (shieldFillImage != null)
        {
            Vector3 s = shieldFillImage.transform.localScale;
            s.x = currentShieldFill;
            shieldFillImage.transform.localScale = s;
        }

        if (showShieldText && shieldText != null)
        {
            shieldText.gameObject.SetActive(true);

            if (usePercentageOfMaxHP)
            {
                int maxHP = team.GetTotalMaxHP();
                int shieldPercent = maxHP > 0 ? Mathf.RoundToInt(((float)shield / maxHP) * 100) : 0;
                shieldText.text = string.Format(shieldFormat, shield);
            }
            else
            {
                shieldText.text = string.Format(shieldFormat, shield);
            }
        }
    }

    /// <summary>
    /// Set max shield cố định (nếu không dùng % max HP)
    /// </summary>
    public void SetFixedMaxShield(int max)
    {
        fixedMaxShield = max;
    }

    // Toggle giữa % max HP hoặc giá trị cố định
    public void SetUsePercentageOfMaxHP(bool usePercentage)
    {
        usePercentageOfMaxHP = usePercentage;
    }

    /// <summary>
    /// Force refresh display ngay lập tức (không animation)
    /// </summary>
    public void RefreshImmediate()
    {
        if (team == null) return;

        int maxHP = team.GetTotalMaxHP();
        int currentHP = team.GetTotalCurrentHP();
        int shield = team.GetTeamShield();

        if (maxHP <= 0) return;

        // Update HP ngay lập tức
        currentHealthFill = (float)currentHP / maxHP;
        if (healthFillImage != null)
        {
            Vector3 s = healthFillImage.transform.localScale;
            s.x = currentHealthFill;
            healthFillImage.transform.localScale = s;
        }

        // Update Shield ngay lập tức
        if (shield > 0)
        {
            if (usePercentageOfMaxHP)
            {
                currentShieldFill = (float)shield / maxHP;
            }
            else
            {
                currentShieldFill = (float)shield / fixedMaxShield;
            }

            if (shieldFillImage != null)
            {
                Vector3 s = shieldFillImage.transform.localScale;
                s.x = Mathf.Clamp01(currentShieldFill);
                shieldFillImage.transform.localScale = s;
            }

            if (shieldCanvasGroup != null)
            {
                shieldCanvasGroup.alpha = 1f;
            }
        }
        else
        {
            if (shieldCanvasGroup != null)
            {
                shieldCanvasGroup.alpha = 0f;
            }
        }
    }

    // Get shield percentage
    public float GetShieldPercentage()
    {
        if (team == null) return 0f;

        int shield = team.GetTeamShield();
        if (shield <= 0) return 0f;

        if (usePercentageOfMaxHP)
        {
            int maxHP = team.GetTotalMaxHP();
            return maxHP > 0 ? ((float)shield / maxHP) * 100f : 0f;
        }
        else
        {
            return ((float)shield / fixedMaxShield) * 100f;
        }
    }
}