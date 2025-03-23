using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item; // The item to pick up, assigned in the Inspector

    void OnTriggerStay2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player detected over item: {item?.name ?? "null"}");
            PickUp(other);
        }
    }

    public void PickUp(Collider2D playerCollider)
    {
        // Get the PlayerInventory component from the player
        PlayerInventory playerInventory = playerCollider.GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("ItemPickUp: PlayerInventory component not found on the player!");
            return;
        }

        // Check if the inventory has space (less than 6 items)
        if (playerInventory.IsInventoryFull())
        {
            Debug.LogWarning($"Cannot pick up item {item?.name ?? "null"}: Inventory is full (6 items).");
            return;
        }

        // Add the item to the inventory
        bool added = playerInventory.PickUpItem(item);
        if (added)
        {
            Debug.Log($"Item {item.name} picked up successfully!");
            Destroy(gameObject); // Remove item from the world
        }
        else
        {
            Debug.LogWarning($"Failed to pick up item {item.name}.");
        }
    }
}