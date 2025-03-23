using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> inventory = new List<Item>();
    public Equipment equipment = new Equipment();

    // Events to notify UI of changes
    public event Action<Item> OnItemAdded;
    public event Action<Item> OnItemRemoved;
    public event Action<ItemType, Item> OnEquipmentChanged;

    public bool IsInventoryFull()
    {
        return inventory.Count >= 6;
    }
    public bool PickUpItem(Item item)
    {
        // Double-check inventory space (redundant with ItemPickUp check, but good practice)
        if (IsInventoryFull())
        {
            Debug.Log("Inventory is full! Cannot pick up item: " + item.name);
            return false;
        }

        // Add item to inventory
        inventory.Add(item);
        OnItemAdded?.Invoke(item);
        Debug.Log($"Item {item.name} added to inventory. Current count: {inventory.Count}");
        return true;
    }

    public void EquipItem(Item item)
    {
        Item unequipped = equipment.Equip(item);
        if (unequipped != null)
        {
            inventory.Add(unequipped);
            OnItemAdded?.Invoke(unequipped);
        }
        inventory.Remove(item);
        OnItemRemoved?.Invoke(item);
        OnEquipmentChanged?.Invoke(item.type, item);
    }

    public void UnequipItem(ItemType type)
    {
        Item unequipped = equipment.Unequip(type);
        if (unequipped != null)
        {
            inventory.Add(unequipped);
            OnItemAdded?.Invoke(unequipped);
            OnEquipmentChanged?.Invoke(type, null);
        }
    }
}