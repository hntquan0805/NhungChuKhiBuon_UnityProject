using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IndividualCPDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI cpText;
    public Image[] cpIcons;

    [Header("Enemy Reference")]
    public EnemyCharacter enemy; // Kéo enemy tương ứng vào đây

    [Header("Display Settings")]
    public string cpFormat = "{0}";
    public Color activeColor = Color.yellow;
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private void Update()
    {
        if (enemy != null)
        {
            UpdateCPDisplay();
        }
    }

    void UpdateCPDisplay()
    {
        int currentCP = enemy.GetCurrentCP();
        int maxCP = enemy.GetMaxCP();

        // Cập nhật text
        if (cpText != null)
        {
            // Kiểm tra nếu là MinionEnemy thì hiển thị "X"
            if (enemy is MinionEnemy)
            {
                cpText.text = "X";
            }
            else
            {
                cpText.text = string.Format(cpFormat, currentCP);
            }
        }

        // Cập nhật icons (nếu có)
        if (cpIcons != null && cpIcons.Length > 0)
        {
            for (int i = 0; i < cpIcons.Length; i++)
            {
                if (cpIcons[i] != null)
                {
                    // Ẩn icon nếu vượt quá maxCP
                    if (i >= maxCP)
                    {
                        cpIcons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        cpIcons[i].gameObject.SetActive(true);
                        // Đổi màu dựa trên CP còn lại
                        cpIcons[i].color = i < currentCP ? activeColor : inactiveColor;
                    }
                }
            }
        }
    }
}