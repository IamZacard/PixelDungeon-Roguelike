using UnityEngine;
using System;

public class Equipment : MonoBehaviour
{
    [Serializable]
    public class EquipmentSlot
    {
        public ItemType slotType;
        public Item equippedItem;
    }

    public EquipmentSlot[] slots; // Array for Helmet, Armor, Gloves, Boots, Sword, Shield
    public delegate void OnEquipmentChanged();
    public OnEquipmentChanged onEquipmentChangedCallback;

    private void Awake()
    {
        slots = new EquipmentSlot[6];
        slots[0] = new EquipmentSlot { slotType = ItemType.Helmet };
        slots[1] = new EquipmentSlot { slotType = ItemType.Armor };
        slots[2] = new EquipmentSlot { slotType = ItemType.Gloves };
        slots[3] = new EquipmentSlot { slotType = ItemType.Boots };
        slots[4] = new EquipmentSlot { slotType = ItemType.Sword };
        slots[5] = new EquipmentSlot { slotType = ItemType.Shield };
    }

    public bool EquipItem(Item item)
    {
        foreach (EquipmentSlot slot in slots)
        {
            if (slot.slotType == item.type)
            {
                if (slot.equippedItem != null)
                {
                    // Swap with inventory or unequip logic here
                    FindObjectOfType<Inventory>().AddItem(slot.equippedItem);
                }
                slot.equippedItem = item;
                onEquipmentChangedCallback?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void UnequipItem(ItemType type)
    {
        foreach (EquipmentSlot slot in slots)
        {
            if (slot.slotType == type && slot.equippedItem != null)
            {
                FindObjectOfType<Inventory>().AddItem(slot.equippedItem);
                slot.equippedItem = null;
                onEquipmentChangedCallback?.Invoke();
                break;
            }
        }
    }
}