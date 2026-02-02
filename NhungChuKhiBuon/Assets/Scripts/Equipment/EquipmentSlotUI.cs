using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI component cho mỗi slot trang bị
/// </summary>
public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image iconImage;
    public Image slotBackground;
    public ItemType slotType;

    [Header("Visual")]
    public Sprite emptySlotSprite;
    public Color emptyColor = new Color(1, 1, 1, 0.3f);
    public Color equippedColor = Color.white;

    private ItemInstance currentItem;
    private string characterName;

    public void Initialize(string charName, ItemType type)
    {
        Debug.Log($"[EquipmentSlotUI] Init {type} for {charName}");
        characterName = charName;
        slotType = type;
        RefreshSlot();
    }

    public void RefreshSlot()
    {
        currentItem = EquipmentManager.Instance.GetEquippedItem(characterName, slotType);

        if (currentItem != null && currentItem.baseData != null)
        {
            // Có trang bị
            iconImage.sprite = currentItem.baseData.icon;
            iconImage.color = equippedColor;

            if (slotBackground != null)
                slotBackground.color = GetTierColor(currentItem.baseData.tier);
        }
        else
        {
            // Slot trống
            iconImage.sprite = emptySlotSprite;
            iconImage.color = emptyColor;

            if (slotBackground != null)
                slotBackground.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[EquipmentSlotUI] CLICK {slotType}");
        if (currentItem != null)
        {
            // Có trang bị -> Tháo ra
            if (EquipmentManager.Instance.UnequipItem(characterName, slotType))
            {
                RefreshSlot();

                // Refresh UI khác nếu cần
                CharacterUpgradeManager upgradeManager = FindObjectOfType<CharacterUpgradeManager>();
                if (upgradeManager != null)
                {
                    upgradeManager.RefreshEquipmentStats();
                }
            }
        }
        else
        {
            // Slot trống -> Mở inventory để chọn trang bị
            EquipmentSelectionPanel selectionPanel = FindObjectOfType<EquipmentSelectionPanel>();
            if (selectionPanel != null)
            {
                selectionPanel.OpenSelection(characterName, slotType, this);
            }
            Debug.Log($"[EquipmentSlotUI] SelectionPanel = {selectionPanel}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.Show(currentItem);
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
                return new Color(0.7f, 0.7f, 0.7f); // Gray
            case ItemTier.Mid:
                return new Color(0.3f, 0.8f, 0.3f); // Green
            case ItemTier.High:
                return new Color(0.8f, 0.3f, 0.8f); // Purple
            default:
                return Color.white;
        }
    }
}