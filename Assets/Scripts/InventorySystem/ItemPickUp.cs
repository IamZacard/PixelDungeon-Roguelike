using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;

    public void PickUp()
    {
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.PickUpItem(item);
            Destroy(gameObject); // Remove item from the world
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }
}