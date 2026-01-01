using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamHealthDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;
    public TextMeshProUGUI healthText;
    public PlayerTeam team;

    [Header("Display Settings")]
    public string healthFormat = "{0}/{1}";
    public bool showText = true;

    private void Update()
    {
        if (team == null)
            return;

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        int maxHP = team.GetTotalMaxHP();
        int currentHP = team.GetTotalCurrentHP();

        // 🔥 CHỐNG CHIA 0 – team chưa sẵn sàng
        if (maxHP <= 0)
            return;

        float ratio = Mathf.Clamp01((float)currentHP / maxHP);

        if (fillImage != null)
        {
            Vector3 s = fillImage.transform.localScale;
            s.x = ratio;
            fillImage.transform.localScale = s;
        }

        if (showText && healthText != null)
        {
            healthText.text = string.Format(healthFormat, currentHP, maxHP);
        }
    }
}
