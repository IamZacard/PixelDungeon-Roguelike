using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public int inventorySize = 20; // Max number of slots
    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChangedCallback;

    public bool AddItem(Item item)
    {
        // Check if stackable and already exists
        if (item.isStackable)
        {
            foreach (Item existingItem in items)
            {
                if (existingItem.id == item.id && items.Count < inventorySize)
                {
                    return true; // Assume stacking logic here if you extend this
                }
            }
        }

        if (items.Count < inventorySize)
        {
            items.Add(item);
            onInventoryChangedCallback?.Invoke();
            return true;
        }
        return false; // Inventory full
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            onInventoryChangedCallback?.Invoke();
        }
    }
}