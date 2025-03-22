using UnityEngine;

public enum ItemType { Helmet, Armor, Gloves, Boots, Sword, Shield, Consumable }

[System.Serializable]
public class Item
{
    public string itemName;
    public ItemType type;
    public Sprite icon; // For inventory UI display
    public int id;
    public bool isStackable;
    public int maxStackSize = 1;

    public Item(string name, ItemType itemType, Sprite sprite, int itemId, bool stackable = false, int maxStack = 1)
    {
        itemName = name;
        type = itemType;
        icon = sprite;
        id = itemId;
        isStackable = stackable;
        maxStackSize = maxStack;
    }
}