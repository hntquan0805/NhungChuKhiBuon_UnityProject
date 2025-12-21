using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class APDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI apText; // Hoặc dùng Text nếu không có TextMeshPro
    public Image[] apIcons; // Mảng icon để hiển thị AP bằng hình ảnh (optional)

    [Header("Display Settings")]
    public string apFormat = "AP: {0}/{1}"; // Format hiển thị
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    private void Update()
    {
        if (BattleManager.Instance != null)
        {
            UpdateAPDisplay();
        }
    }

    void UpdateAPDisplay()
    {
        int currentAP = BattleManager.Instance.context.playerAP;
        int maxAP = BattleManager.Instance.playerMaxAP;

        // Cập nhật text
        if (apText != null)
        {
            apText.text = string.Format(apFormat, currentAP, maxAP);
        }

        // Cập nhật icons (nếu có)
        if (apIcons != null && apIcons.Length > 0)
        {
            for (int i = 0; i < apIcons.Length; i++)
            {
                if (apIcons[i] != null)
                {
                    // Ẩn icon nếu vượt quá maxAP
                    if (i >= maxAP)
                    {
                        apIcons[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        apIcons[i].gameObject.SetActive(true);
                        // Đổi màu dựa trên AP còn lại
                        apIcons[i].color = i < currentAP ? activeColor : inactiveColor;
                    }
                }
            }
        }
    }
}