using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform content; // Assign the ScrollView's "Content" in the inspector
    private PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerInventory.OnItemAdded += AddItemSlot;
        playerInventory.OnItemRemoved += RemoveItemSlot;
        // Populate initial inventory
        foreach (var item in playerInventory.inventory)
        {
            AddItemSlot(item);
        }
    }

    void AddItemSlot(Item item)
    {
        GameObject slot = Instantiate(slotPrefab, content);
        slot.GetComponentInChildren<Image>().sprite = item.icon;
        slot.GetComponent<Button>().onClick.AddListener(() => OnItemSelected(item));
    }

    void RemoveItemSlot(Item item)
    {
        foreach (Transform child in content)
        {
            ItemSlot slot = child.GetComponent<ItemSlot>();
            if (slot != null && slot.item == item)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    void OnItemSelected(Item item)
    {
        // Simple example: Equip the item directly
        // In a real game, show a context menu with "Equip"
        playerInventory.EquipItem(item);
    }
}

// Attach this to each slot to reference its item
public class ItemSlot : MonoBehaviour
{
    public Item item;
}