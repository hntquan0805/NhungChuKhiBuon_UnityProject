using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject shopPanel;

    [Header("Buttons")]
    public Button shopButton;
    public Button closeButton;
    public Button refreshButton;

    [Header("Item Data")]
    public List<ItemData> allItems;

    [Header("Item Slots")]
    public List<ShopItemSlot> itemSlots;

    [Header("Shop Cost")]
    public int refreshCost = 50;

    private bool shopGenerated = false;

    public static ShopManager Instance { get; private set; }


    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        shopButton.onClick.AddListener(OpenShop);
        closeButton.onClick.AddListener(CloseShop);
        refreshButton.onClick.AddListener(RefreshShop);

        shopPanel.SetActive(false);
    }

    void OpenShop()
    {
        // Phát âm thanh mở shop
        if (AudioRestAreaManager.Instance != null)
        {
            AudioRestAreaManager.Instance.PlayButtonClick();
        }

        shopPanel.SetActive(true);
        if (!shopGenerated)
        {
            GenerateShopItems();
            shopGenerated = true;
        }
    }

    void CloseShop()
    {
        // Phát âm thanh đóng shop
        if (AudioRestAreaManager.Instance != null)
        {
            AudioRestAreaManager.Instance.PlayButtonClick();
        }

        shopPanel.SetActive(false);
    }

    void RefreshShop()
    {
        int playerCoins = MenuManager.Instance.PlayerCoins;

        bool success = MenuManager.Instance.SpendCoins(refreshCost);

        if (!success) {
            Debug.Log("Not enough coins");

            // Phát âm thanh không đủ tiền
            if (AudioRestAreaManager.Instance != null)
            {
                AudioRestAreaManager.Instance.PlayError();
            }
            return;
        }

        if (AudioRestAreaManager.Instance != null)
        {
            AudioRestAreaManager.Instance.PlayButtonClick();
        }

        Debug.Log("Refresh shop");
        GenerateShopItems();
    }

    public void ResetShop()
    {
        shopGenerated = false;
    }


    void GenerateShopItems()
    {
        List<ItemData> randomItems = GetRandomItems(itemSlots.Count);

        // Reset slot (không giảm giá)
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].SetItem(randomItems[i]);
        }

        // ---- DISCOUNT LOGIC ----
        int discountCount = Random.Range(0, 3); // 0, 1 hoặc 2
        List<int> usedIndexes = new List<int>();

        float[] discountRates = { 0.1f, 0.2f, 0.3f };

        for (int i = 0; i < discountCount; i++)
        {
            int index;
            do
            {
                index = Random.Range(0, itemSlots.Count);
            }
            while (usedIndexes.Contains(index));

            usedIndexes.Add(index);

            float rate = discountRates[Random.Range(0, discountRates.Length)];
            itemSlots[index].SetItem(randomItems[index], rate);
        }
    }


    List<ItemData> GetRandomItems(int count)
    {
        List<ItemData> result = new List<ItemData>();

        for (int i = 0; i < count; i++)
        {
            ItemData item = allItems[Random.Range(0, allItems.Count)];
            result.Add(item);
        }

        return result;
    }


    public void UpdateAllSlots()
    {
        int coins = MenuManager.Instance.PlayerCoins;

        foreach (var slot in itemSlots)
        {
            slot.UpdateInteractable(coins);
        }
    }

}