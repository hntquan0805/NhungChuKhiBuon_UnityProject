using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;
    public TMP_Text nameText;
    public TMP_Text mainStatText;
    public TMP_Text subStatText;

    RectTransform rectTransform;
    Canvas canvas;

    void Awake()
    {
        // Singleton đơn giản, không dùng DontDestroyOnLoad
        Instance = this;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Tooltip follow chuột với clamp
        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector2 offset = new Vector2(15, -15);
        Vector2 targetPosition = (Vector2)Input.mousePosition + offset;

        // Lấy kích thước tooltip
        Vector2 tooltipSize = rectTransform.sizeDelta;

        // Lấy kích thước màn hình
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Tính scale của Canvas (nếu có Canvas Scaler)
        float canvasScale = canvas != null ? canvas.scaleFactor : 1f;

        // Tính kích thước thực tế của tooltip trên màn hình
        Vector2 scaledSize = tooltipSize * canvasScale;

        // Clamp vị trí để không vượt ra ngoài màn hình
        // Kiểm tra cạnh phải
        if (targetPosition.x + scaledSize.x > screenWidth)
        {
            targetPosition.x = Input.mousePosition.x - scaledSize.x - 15; // Đổi sang bên trái chuột
        }

        // Kiểm tra cạnh trái
        if (targetPosition.x < 0)
        {
            targetPosition.x = 15;
        }

        // Kiểm tra cạnh dưới
        if (targetPosition.y - scaledSize.y < 0)
        {
            targetPosition.y = Input.mousePosition.y + scaledSize.y + 15; // Đổi sang trên chuột
        }

        // Kiểm tra cạnh trên
        if (targetPosition.y > screenHeight)
        {
            targetPosition.y = screenHeight - 15;
        }

        rectTransform.position = targetPosition;
    }

    public void Show(ItemInstance item)
    {
        nameText.text = item.baseData.itemName;
        mainStatText.text = $"{item.mainStat.statType} +{item.mainStat.value}";

        subStatText.text = "";
        foreach (var sub in item.subStats)
        {
            subStatText.text += $"{sub.statType} +{sub.value}\n";
        }

        gameObject.SetActive(true);

        // Force rebuild để có size chính xác ngay lập tức
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}