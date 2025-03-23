using UnityEngine;

public class Equipment
{
    public Item helmet;
    public Item coreArmor;
    public Item hands;
    public Item boots;
    public Item sword;
    public Item shield;

    public int GetTotalAttackBonus()
    {
        int total = 0;
        if (helmet != null) total += helmet.attackBonus;
        if (coreArmor != null) total += coreArmor.attackBonus;
        if (hands != null) total += hands.attackBonus;
        if (boots != null) total += boots.attackBonus;
        if (sword != null) total += sword.attackBonus;
        if (shield != null) total += shield.attackBonus;
        return total;
    }

    public int GetTotalDefenseBonus()
    {
        int total = 0;
        if (helmet != null) total += helmet.defenseBonus;
        if (coreArmor != null) total += coreArmor.defenseBonus;
        if (hands != null) total += hands.defenseBonus;
        if (boots != null) total += boots.defenseBonus;
        if (sword != null) total += sword.attackBonus;
        if (shield != null) total += shield.defenseBonus;
        return total;
    }

    public int GetTotalHealthBonus()
    {
        int total = 0;
        if (helmet != null) total += helmet.healthBonus;
        if (coreArmor != null) total += coreArmor.healthBonus;
        if (hands != null) total += hands.healthBonus;
        if (boots != null) total += boots.healthBonus;
        if (sword != null) total += sword.healthBonus;
        if (shield != null) total += shield.healthBonus;
        return total;
    }

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