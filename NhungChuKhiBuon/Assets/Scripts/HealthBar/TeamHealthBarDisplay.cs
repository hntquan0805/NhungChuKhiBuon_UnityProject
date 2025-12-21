using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamHealthDisplay : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage; // Thanh máu
    public TextMeshProUGUI healthText; // Text hiển thị số máu (optional)
    public PlayerTeam team;

    [Header("Display Settings")]
    public string healthFormat = "{0}/{1}"; // Format: CurrentHP/MaxHP
    public bool showText = true; // Có hiển thị text hay không

    private float maxHP;

    private void Start()
    {
        if (team != null)
        {
            maxHP = team.GetTotalMaxHP();
        }
    }

    private void Update()
    {
        if (team != null)
        {
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        int currentHP = team.GetTotalCurrentHP();
        
        // Cập nhật thanh máu
        if (fillImage != null)
        {
            Vector3 s = fillImage.transform.localScale;
            s.x = (float)currentHP / (float)maxHP;
            fillImage.transform.localScale = s;
        }

        // Cập nhật text (nếu có)
        if (showText && healthText != null)
        {
            healthText.text = string.Format(healthFormat, currentHP, (int)maxHP);
        }
    }
}