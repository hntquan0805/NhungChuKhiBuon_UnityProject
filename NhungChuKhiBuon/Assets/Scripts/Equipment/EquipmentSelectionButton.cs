using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// Button để chọn item từ inventory
/// </summary>
public class EquipmentSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public Image backgroundImage;
    public Button selectButton;

    private ItemInstance item;
    private EquipmentSelectionPanel panel;

    public void Initialize(ItemInstance itemInstance, EquipmentSelectionPanel selectionPanel)
    {
        item = itemInstance;
        panel = selectionPanel;

        // Set icon
        if (itemIcon != null && item.baseData.icon != null)
        {
            itemIcon.sprite = item.baseData.icon;
        }

        // Set name
        if (itemNameText != null)
        {
            itemNameText.text = item.baseData.itemName;
        }

        // Set background color based on tier
        if (backgroundImage != null)
        {
            backgroundImage.color = GetTierColor(item.baseData.tier);
        }

        // Setup button
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectClicked);
        }
    }

    private void OnSelectClicked()
    {
        Debug.Log($"[EquipButton] CLICK item = {item?.baseData?.itemName}");

        if (panel != null)
        {
            Debug.Log("[EquipButton] Panel OK → gọi SelectItem");
            panel.SelectItem(item);
        }
        else
        {
            Debug.LogError("[EquipButton] panel == NULL");
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.Show(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.Hide();
        }
    }

    private Color GetTierColor(ItemTier tier)
    {
        switch (tier)
        {
            case ItemTier.Basic:
                return new Color(0.7f, 0.7f, 0.7f, 0.5f); // Gray
            case ItemTier.Mid:
                return new Color(0.3f, 0.8f, 0.3f, 0.5f); // Green
            case ItemTier.High:
                return new Color(0.8f, 0.3f, 0.8f, 0.5f); // Purple
            default:
                return Color.white;
        }
    }
}