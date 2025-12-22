using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public Transform contentParent;
    public GameObject inventoryPanel;

    void OnEnable()
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshInventory;
        RefreshInventory();
    }

    void OnDisable()
    {
        InventoryManager.Instance.OnInventoryChanged -= RefreshInventory;
    }

    public void RefreshInventory()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager not found!");
            return;
        }

        if (inventorySlotPrefab == null || contentParent == null)
        {
            Debug.LogError("Inventory UI references not set!");
            return;
        }

        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        foreach (var item in InventoryManager.Instance.GetItems())
        {
            GameObject slot = Instantiate(inventorySlotPrefab, contentParent);
            slot.GetComponent<InventorySlot>().SetItem(item);
        }
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

}
