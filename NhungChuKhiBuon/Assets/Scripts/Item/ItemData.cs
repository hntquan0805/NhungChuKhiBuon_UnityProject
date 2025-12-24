using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Shop/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public ItemTier tier;
    public ItemType type;
    public ItemSet set;
    public int price;
    public Sprite icon;
}
