using UnityEngine;
using TMPro; // Dành cho TextMeshPro
using UnityEngine.UI; // Dành cho Text thường

public class BlinkingEffect : MonoBehaviour
{
    [Header("Cài đặt")]
    public float speed = 1f;      // Tốc độ nhấp nháy
    public float minAlpha = 0.2f; // Độ mờ thấp nhất (0 là mất hẳn, 0.2 là mờ mờ)
    public float maxAlpha = 1f;   // Độ rõ cao nhất (1 là rõ hoàn toàn)

    private TextMeshProUGUI tmpText;
    private Text legacyText;

    void Start()
    {
        // Tự động tìm xem bạn đang dùng loại Text nào
        tmpText = GetComponent<TextMeshProUGUI>();
        legacyText = GetComponent<Text>();
    }

    void Update()
    {
        // Tính toán độ Alpha theo hình Sin (lên xuống nhịp nhàng)
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f);

        // Áp dụng màu mới
        if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = alpha;
            tmpText.color = c;
        }
        else if (legacyText != null)
        {
            Color c = legacyText.color;
            c.a = alpha;
            legacyText.color = c;
        }
    }
}