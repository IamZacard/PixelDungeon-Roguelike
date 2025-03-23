using UnityEngine;

public class Equipment
{
    public Item helmet;
    public Item coreArmor;
    public Item hands;
    public Item boots;
    public Item sword;
    public Item shield;

    public Item Equip(Item item)
    {
        // Check if the item is equippable (optional, but good practice)
        if (!IsEquippable(item.type)) return null;

        Item currentItem = null;
        switch (item.type)
        {
            case ItemType.Helmet:
                currentItem = helmet;
                helmet = item;
                break;
            case ItemType.CoreArmor:
                currentItem = coreArmor;
                coreArmor = item;
                break;
            case ItemType.Hands:
                currentItem = hands;
                hands = item;
                break;
            case ItemType.Boots:
                currentItem = boots;
                boots = item;
                break;
            case ItemType.Sword:
                currentItem = sword;
                sword = item;
                break;
            case ItemType.Shield:
                currentItem = shield;
                shield = item;
                break;
        }
        return currentItem; // Return the previously equipped item
    }

    public Item Unequip(ItemType type)
    {
        Item currentItem = null;
        switch (type)
        {
            case ItemType.Helmet:
                currentItem = helmet;
                helmet = null;
                break;
            case ItemType.CoreArmor:
                currentItem = coreArmor;
                coreArmor = null;
                break;
            case ItemType.Hands:
                currentItem = hands;
                hands = null;
                break;
            case ItemType.Boots:
                currentItem = boots;
                boots = null;
                break;
            case ItemType.Sword:
                currentItem = sword;
                sword = null;
                break;
            case ItemType.Shield:
                currentItem = shield;
                shield = null;
                break;
        }
        return currentItem;
    }

    private bool IsEquippable(ItemType type)
    {
        return type == ItemType.Helmet || type == ItemType.CoreArmor || type == ItemType.Hands ||
               type == ItemType.Boots || type == ItemType.Sword || type == ItemType.Shield;
    }
}