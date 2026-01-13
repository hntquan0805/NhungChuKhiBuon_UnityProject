using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text discountText;
    public GameObject soldOutOverlay;

    private ItemInstance currentItemInstance;
    private int finalPrice;
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(BuyItem);
    }

    // ===== SET ITEM (SINH STAT TẠI ĐÂY) =====
    public void SetItem(ItemData itemData, float discountRate = 0f)
    {
        // 1️⃣ Sinh item NGAY KHI LÊN SHOP
        currentItemInstance = ItemGenerator.CreateItem(itemData);

        // 2️⃣ UI cơ bản
        icon.sprite = itemData.icon;
        nameText.text = itemData.itemName;

        // 3️⃣ Giá + giảm giá
        if (discountRate > 0f)
        {
            finalPrice = Mathf.RoundToInt(itemData.price * (1f - discountRate));
            priceText.text = finalPrice.ToString();
            priceText.color = Color.red;
            discountText.text = "-" + (discountRate * 100).ToString("0") + "%";
            discountText.gameObject.SetActive(true);
        }
        else
        {
            finalPrice = itemData.price;
            priceText.text = finalPrice.ToString();
            priceText.color = Color.yellow;
            discountText.gameObject.SetActive(false);
        }

        // 4️⃣ Reset trạng thái
        soldOutOverlay.SetActive(false);
        button.interactable = true;

        // (Optional) Debug test
        Debug.Log($"[SHOP] {itemData.itemName} | Main: {currentItemInstance.mainStat.statType} +{currentItemInstance.mainStat.value}");
    }

    public int GetFinalPrice()
    {
        return finalPrice;
    }

    // ===== BUY =====
    void BuyItem()
    {
        if (currentItemInstance == null) return;

        bool success = MenuManager.Instance.SpendCoins(finalPrice);
        if (!success)
        {
            Debug.Log("Not enough coins");

            // Phát âm thanh không đủ tiền
            if (AudioRestAreaManager.Instance != null)
            {
                AudioRestAreaManager.Instance.PlayError();
            }
            return;
        }

        // Phát âm thanh mua thành công
        if (AudioRestAreaManager.Instance != null)
        {
            AudioRestAreaManager.Instance.PlayPurchase();
        }

        // Add ĐÚNG item đã xem stat
        InventoryManager.Instance.AddItem(currentItemInstance);
        Debug.Log("Bought item: " + currentItemInstance.baseData.itemName);

        // Khoá slot
        button.interactable = false;
        soldOutOverlay.SetActive(true);

        // Ẩn tooltip khi mua xong
        if (TooltipManager.Instance != null)
            TooltipManager.Instance.Hide();

        // Update các slot khác (xám nếu thiếu coin)
        ShopManager.Instance.UpdateAllSlots();
    }

    // ===== UPDATE INTERACTABLE =====
    public void UpdateInteractable(int playerCoins)
    {
        if (soldOutOverlay.activeSelf) return;
        button.interactable = playerCoins >= finalPrice;
    }

    // ===== TOOLTIP HOVER =====
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItemInstance != null && TooltipManager.Instance != null)
        {
            TooltipManager.Instance.Show(currentItemInstance);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.Hide();
        }
    }
}