using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;

    private ItemInstance currentItem;

    public void SetItem(ItemInstance item)
    {
        currentItem = item;
        icon.sprite = item.baseData.icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null)
            TooltipManager.Instance.Show(currentItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.Instance.Hide();
    }
}
