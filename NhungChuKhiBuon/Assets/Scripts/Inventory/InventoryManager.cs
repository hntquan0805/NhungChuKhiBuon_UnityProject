using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private List<ItemInstance> items = new List<ItemInstance>();

    public event System.Action OnInventoryChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(ItemInstance item)
    {
        if (item == null) return;

        items.Add(item);
        Debug.Log("Inventory +1: " + item.baseData.itemName);

        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemInstance item)
    {
        if (items.Remove(item))
        {
            OnInventoryChanged?.Invoke();
        }
    }

    public List<ItemInstance> GetItems()
    {
        return items;
    }
}
