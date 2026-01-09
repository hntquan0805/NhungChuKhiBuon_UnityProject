using UnityEngine;
using TMPro; // Dành cho TextMeshPro
using UnityEngine.UI; // Dành cho Text thường

public class BlinkingEffect : MonoBehaviour
{
    [Header("Cài đặt")]
    public float speed = 1f;
    public float minAlpha = 0.2f;
    public float maxAlpha = 1f;

    private TextMeshProUGUI tmpText;
    private Text legacyText;

    void Start()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        legacyText = GetComponent<Text>();
    }

    void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f);

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