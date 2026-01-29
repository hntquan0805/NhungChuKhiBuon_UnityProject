using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Panel để chọn trang bị từ inventory
/// </summary>
public class EquipmentSelectionPanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Transform contentParent;
    public GameObject itemButtonPrefab;
    public Button closeButton;

    private string currentCharacterName;
    private ItemType currentSlotType;
    private EquipmentSlotUI currentSlot;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseSelection);
        }

        panel.SetActive(false);
    }

    public void OpenSelection(string characterName, ItemType slotType, EquipmentSlotUI slot)
    {
        currentCharacterName = characterName;
        currentSlotType = slotType;
        currentSlot = slot;

        RefreshItemList();
        panel.SetActive(true);
    }

    public void CloseSelection()
    {
        panel.SetActive(false);
    }

    private void RefreshItemList()
    {
        // Clear old buttons
        foreach (var button in spawnedButtons)
        {
            if (button != null)
                Destroy(button);
        }
        spawnedButtons.Clear();

        // ===== DEBUG START =====
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[EquipPanel] InventoryManager.Instance == NULL");
            return;
        }

        List<ItemInstance> allItems = InventoryManager.Instance.GetItems();
        Debug.Log($"[EquipPanel] Inventory total = {allItems.Count}");
        Debug.Log($"[EquipPanel] Current Slot Type = {currentSlotType}");

        foreach (var item in allItems)
        {
            if (item == null)
            {
                Debug.LogError("[EquipPanel] ItemInstance is NULL");
                continue;
            }

            if (item.baseData == null)
            {
                Debug.LogError("[EquipPanel] baseData is NULL");
                continue;
            }

            Debug.Log($"[EquipPanel] Item = {item.baseData.itemName}, type = {item.baseData.type}");
        }
        // ===== DEBUG END =====

        // Get items from inventory that match the slot type
        List<ItemInstance> availableItems = allItems
            .FindAll(item => item.baseData != null && item.baseData.type == currentSlotType);

        Debug.Log($"[EquipPanel] Available items for {currentSlotType} = {availableItems.Count}");

        // Create button for each item
        foreach (var item in availableItems)
        {
            GameObject buttonObj = Instantiate(itemButtonPrefab, contentParent);
            EquipmentSelectionButton button = buttonObj.GetComponent<EquipmentSelectionButton>();

            if (button != null)
            {
                button.Initialize(item, this);
            }
            else
            {
                Debug.LogError("[EquipPanel] EquipmentSelectionButton missing on prefab");
            }

            spawnedButtons.Add(buttonObj);
        }
    }


    public void SelectItem(ItemInstance item)
    {
        Debug.Log($"[EquipPanel] SelectItem = {item?.baseData?.itemName}");

        bool success = EquipmentManager.Instance.EquipItem(currentCharacterName, item);
        Debug.Log($"[EquipPanel] Equip result = {success}");

        if (success)
        {
            if (currentSlot != null)
            {
                Debug.Log("[EquipPanel] Refresh slot UI");
                currentSlot.RefreshSlot();
            }
            else
            {
                Debug.LogError("[EquipPanel] currentSlot == NULL");
            }
            if (TooltipManager.Instance != null)
            {
                TooltipManager.Instance.Hide();
            }
            CharacterUpgradeManager upgradeManager = FindObjectOfType<CharacterUpgradeManager>();
            if (upgradeManager != null)
            {
                upgradeManager.RefreshEquipmentStats(); // Hàm này sẽ tính lại Base + Bonus và UpdateUI
            }

            CloseSelection();
        }
    }
}